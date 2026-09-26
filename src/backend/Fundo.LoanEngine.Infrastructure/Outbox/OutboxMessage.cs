namespace Fundo.LoanEngine.Infrastructure.Outbox;

public enum OutboxMessageStatus
{
    Pending,
    Processing,
    Processed,
    Failed
}

public sealed class OutboxMessage
{
    private OutboxMessage() { }

    private OutboxMessage(string eventType, string payload, DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid();
        EventType = eventType;
        Payload = payload;
        Status = OutboxMessageStatus.Pending;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string EventType { get; private set; } = null!;
    public string Payload { get; private set; } = null!;
    public OutboxMessageStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? NextAttemptAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public DateTimeOffset? LockedUntil { get; private set; }
    public string? LastError { get; private set; }

    public static OutboxMessage Create(string eventType, string payload, DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        return new OutboxMessage(eventType, payload, createdAt);
    }

    public void MarkProcessing(DateTimeOffset lockedUntil)
    {
        Status = OutboxMessageStatus.Processing;
        LockedUntil = lockedUntil;
    }

    public void MarkProcessed(DateTimeOffset processedAt)
    {
        Status = OutboxMessageStatus.Processed;
        ProcessedAt = processedAt;
        LockedUntil = null;
        NextAttemptAt = null;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        RetryCount++;
        Status = OutboxMessageStatus.Failed;
        LockedUntil = null;
        LastError = error;
    }

    public void ScheduleRetry(string error, DateTimeOffset nextAttemptAt)
    {
        RetryCount++;
        Status = OutboxMessageStatus.Pending;
        LockedUntil = null;
        NextAttemptAt = nextAttemptAt;
        LastError = error;
    }
}