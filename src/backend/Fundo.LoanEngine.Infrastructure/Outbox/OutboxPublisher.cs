using System.Net;
using System.Net.Http;
using System.Text.Json;
using Fundo.LoanEngine.Application.Interfaces;
using Fundo.LoanEngine.Application.Messaging;
using Fundo.LoanEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fundo.LoanEngine.Infrastructure.Outbox;

public sealed class OutboxPublisher(
    ApplicationDbContext db,
    IExternalMockClient externalMockClient,
    ILogger<OutboxPublisher> logger,
    IClock clock)
{
    private const int BatchSize = 20;
    private const int MaximumRetries = 10;
    private static readonly TimeSpan MessageLease = TimeSpan.FromMinutes(1);

    public async Task ProcessPendingAsync(CancellationToken cancellationToken)
    {
        var messages = await ClaimMessagesAsync(cancellationToken);

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ProcessMessageAsync(message, cancellationToken);
        }
    }

    private async Task<List<OutboxMessage>> ClaimMessagesAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var messages = await db.OutboxMessages
            .FromSqlInterpolated($"""
                SELECT *
                FROM outbox_messages
                WHERE (status = 'Pending' AND (next_attempt_at IS NULL OR next_attempt_at <= now()))
                   OR (status = 'Processing' AND locked_until <= now())
                ORDER BY created_at, id
                LIMIT {BatchSize}
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(cancellationToken);

        var lockedUntil = clock.UtcNow.Add(MessageLease);
        foreach (var message in messages)
        {
            message.MarkProcessing(lockedUntil);
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return messages;
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.Equals(message.EventType, nameof(ApplicationSubmittedEvent), StringComparison.Ordinal))
            {
                message.MarkFailed($"Unsupported outbox event type '{message.EventType}'.");
            }
            else
            {
                var submittedEvent = JsonSerializer.Deserialize<ApplicationSubmittedEvent>(message.Payload)
                    ?? throw new JsonException("Outbox event payload was empty.");
                await externalMockClient.SendApplicationAsync(submittedEvent, cancellationToken);
                message.MarkProcessed(clock.UtcNow);
                logger.LogInformation("Processed outbox message {OutboxMessageId}.", message.Id);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpRequestException exception) when (exception.StatusCode is >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError)
        {
            var error = $"External service returned HTTP {(int)exception.StatusCode.Value}.";
            message.MarkFailed(error);
            logger.LogError("Outbox message {OutboxMessageId} was rejected with HTTP {StatusCode}.", message.Id, (int)exception.StatusCode.Value);
        }
        catch (Exception exception)
        {
            var error = exception is HttpRequestException httpException && httpException.StatusCode is not null
                ? $"External service returned HTTP {(int)httpException.StatusCode.Value}."
                : $"Outbox processing failed ({exception.GetType().Name}).";

            if (message.RetryCount + 1 >= MaximumRetries)
            {
                message.MarkFailed(error);
                logger.LogError("Outbox message {OutboxMessageId} exhausted its retries after {ExceptionType}.", message.Id, exception.GetType().Name);
            }
            else
            {
                var retryCount = message.RetryCount + 1;
                var delaySeconds = Math.Min(Math.Pow(2, retryCount), 300);
                message.ScheduleRetry(error, clock.UtcNow.AddSeconds(delaySeconds));
                logger.LogWarning("Outbox message {OutboxMessageId} will retry after attempt {RetryCount} ({ExceptionType}).", message.Id, retryCount, exception.GetType().Name);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}