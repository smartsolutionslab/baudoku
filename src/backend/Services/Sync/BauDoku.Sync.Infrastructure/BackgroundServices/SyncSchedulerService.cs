using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BauDoku.Sync.Infrastructure.BackgroundServices;

public sealed class SyncSchedulerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyncSchedulerService> _logger;
    private readonly TimeSpan _interval;

    public SyncSchedulerService(
        IServiceScopeFactory scopeFactory,
        ILogger<SyncSchedulerService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var seconds = configuration.GetValue("Sync:SchedulerIntervalSeconds", 30);
        _interval = TimeSpan.FromSeconds(seconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SyncScheduler gestartet. Intervall: {Interval}s", _interval.TotalSeconds);

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
                _logger.LogError(ex, "Fehler im SyncScheduler-Zyklus");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("SyncScheduler beendet");
    }

    private async Task ProcessPendingBatchesAsync(CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISyncBatchRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var pendingBatches = await repository.GetPendingBatchesAsync(limit: 10, ct);

        if (pendingBatches.Count == 0)
            return;

        _logger.LogInformation("SyncScheduler: {Count} ausstehende Batches gefunden", pendingBatches.Count);

        foreach (var batch in pendingBatches)
        {
            try
            {
                batch.MarkCompleted();
                _logger.LogInformation("SyncScheduler: Batch {BatchId} verarbeitet", batch.Id.Value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SyncScheduler: Fehler bei Batch {BatchId}", batch.Id.Value);
                batch.MarkFailed();
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
