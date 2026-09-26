using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fundo.LoanEngine.Infrastructure.Outbox;

public sealed class OutboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PollInterval);

        do
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var publisher = scope.ServiceProvider.GetRequiredService<OutboxPublisher>();
                await publisher.ProcessPendingAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while processing the transactional outbox.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}