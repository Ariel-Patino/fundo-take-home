using Fundo.LoanEngine.Infrastructure.Outbox;
using Xunit;

namespace Fundo.LoanEngine.Tests.Services;

public sealed class OutboxMessageTests
{
    [Fact]
    public void ScheduleRetry_WhenProcessingFails_IncrementsRetryAndReturnsToPending()
    {
        var message = OutboxMessage.Create("ApplicationSubmittedEvent", "{}", DateTimeOffset.UtcNow);

        message.MarkProcessing(DateTimeOffset.UtcNow.AddMinutes(1));
        message.ScheduleRetry("Temporary failure", DateTimeOffset.UtcNow.AddSeconds(2));

        Assert.Equal(OutboxMessageStatus.Pending, message.Status);
        Assert.Equal(1, message.RetryCount);
        Assert.Null(message.LockedUntil);
        Assert.Equal("Temporary failure", message.LastError);
    }

    [Fact]
    public void MarkProcessed_WhenDeliverySucceeds_ClearsRetryData()
    {
        var message = OutboxMessage.Create("ApplicationSubmittedEvent", "{}", DateTimeOffset.UtcNow);
        message.ScheduleRetry("Temporary failure", DateTimeOffset.UtcNow.AddSeconds(2));

        var processedAt = DateTimeOffset.UtcNow;
        message.MarkProcessed(processedAt);

        Assert.Equal(OutboxMessageStatus.Processed, message.Status);
        Assert.Equal(processedAt, message.ProcessedAt);
        Assert.Null(message.NextAttemptAt);
        Assert.Null(message.LastError);
    }
}