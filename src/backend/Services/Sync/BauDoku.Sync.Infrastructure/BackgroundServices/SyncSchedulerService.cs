using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Persistence;
using SmartSolutionsLab.BauDoku.Sync.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SmartSolutionsLab.BauDoku.Sync.Infrastructure.BackgroundServices;

public sealed partial class SyncSchedulerService(
    IServiceScopeFactory scopeFactory,
    ILogger<SyncSchedulerService> logger,
    IOptions<SyncOptions> syncOptions) : BackgroundService
{
    private readonly TimeSpan interval = TimeSpan.FromSeconds(syncOptions.Value.SchedulerIntervalSeconds);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogSchedulerStarted(interval.TotalSeconds);

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
                LogSchedulerCycleError(ex);
            }

            await Task.Delay(interval, stoppingToken);
        }

        LogSchedulerStopped();
    }

    private async Task ProcessPendingBatchesAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var syncBatches = scope.ServiceProvider.GetRequiredService<ISyncBatchRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var pendingBatches = await syncBatches.GetPendingBatchesAsync(limit: syncOptions.Value.PendingBatchLimit, cancellationToken);

        if (pendingBatches.Count == 0)
            return;

        LogPendingBatchesFound(pendingBatches.Count);

        foreach (var batch in pendingBatches)
        {
            try
            {
                // TODO: BD-706 — Implement actual batch processing (delta application, conflict detection, version store updates)
                LogBatchCompletedPlaceholder(batch.Id.Value);
                batch.MarkCompleted();
            }
            catch (Exception ex)
            {
                LogBatchProcessingError(ex, batch.Id.Value);
                batch.MarkFailed();
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    [LoggerMessage(EventId = 3001, Level = LogLevel.Information,
        Message = "SyncScheduler gestartet. Intervall: {IntervalSeconds}s")]
    private partial void LogSchedulerStarted(double intervalSeconds);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Error,
        Message = "Fehler im SyncScheduler-Zyklus")]
    private partial void LogSchedulerCycleError(Exception exception);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Information,
        Message = "SyncScheduler beendet")]
    private partial void LogSchedulerStopped();

    [LoggerMessage(EventId = 3004, Level = LogLevel.Information,
        Message = "SyncScheduler: {Count} ausstehende Batches gefunden")]
    private partial void LogPendingBatchesFound(int count);

    [LoggerMessage(EventId = 3005, Level = LogLevel.Warning,
        Message = "SyncScheduler: Batch {BatchId} als abgeschlossen markiert (Platzhalter — keine Delta-Verarbeitung)")]
    private partial void LogBatchCompletedPlaceholder(Guid batchId);

    [LoggerMessage(EventId = 3006, Level = LogLevel.Warning,
        Message = "SyncScheduler: Fehler bei Batch {BatchId}")]
    private partial void LogBatchProcessingError(Exception exception, Guid batchId);
}
