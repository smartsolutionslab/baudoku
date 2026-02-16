using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BauDoku.Sync.Infrastructure.BackgroundServices;

public sealed class SyncSchedulerService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<SyncSchedulerService> logger;
    private readonly TimeSpan interval;

    public SyncSchedulerService(
        IServiceScopeFactory scopeFactory,
        ILogger<SyncSchedulerService> logger,
        IConfiguration configuration)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;

        var seconds = configuration.GetValue("Sync:SchedulerIntervalSeconds", 30);
        interval = TimeSpan.FromSeconds(seconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SyncScheduler gestartet. Intervall: {Interval}s", interval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingBatchesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler im SyncScheduler-Zyklus");
            }

            await Task.Delay(interval, stoppingToken);
        }

        logger.LogInformation("SyncScheduler beendet");
    }

    private async Task ProcessPendingBatchesAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var syncBatches = scope.ServiceProvider.GetRequiredService<ISyncBatchRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var pendingBatches = await syncBatches.GetPendingBatchesAsync(limit: 10, ct);

        if (pendingBatches.Count == 0)
            return;

        logger.LogInformation("SyncScheduler: {Count} ausstehende Batches gefunden", pendingBatches.Count);

        foreach (var batch in pendingBatches)
        {
            try
            {
                // TODO: BD-706 — Implement actual batch processing (delta application, conflict detection, version store updates)
                logger.LogWarning(
                    "SyncScheduler: Batch {BatchId} als abgeschlossen markiert (Platzhalter — keine Delta-Verarbeitung)",
                    batch.Id.Value);
                batch.MarkCompleted();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "SyncScheduler: Fehler bei Batch {BatchId}", batch.Id.Value);
                batch.MarkFailed();
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
