namespace BauDoku.Sync.Infrastructure.BackgroundServices;

public sealed class SyncOptions
{
    public int SchedulerIntervalSeconds { get; set; } = 30;
}
