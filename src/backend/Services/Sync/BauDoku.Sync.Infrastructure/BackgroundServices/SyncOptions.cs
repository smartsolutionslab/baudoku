namespace SmartSolutionsLab.BauDoku.Sync.Infrastructure.BackgroundServices;

public sealed class SyncOptions
{
    public int SchedulerIntervalSeconds { get; set; } = 30;
    public int PendingBatchLimit { get; set; } = 10;
}
