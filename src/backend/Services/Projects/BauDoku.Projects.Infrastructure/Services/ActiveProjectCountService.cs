using SmartSolutionsLab.BauDoku.Projects.Application.Contracts;
using SmartSolutionsLab.BauDoku.Projects.Application.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SmartSolutionsLab.BauDoku.Projects.Infrastructure.Services;

public sealed partial class ActiveProjectCountService(IServiceScopeFactory scopeFactory, ILogger<ActiveProjectCountService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(60);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var provider = scope.ServiceProvider.GetRequiredService<IProjectCountProvider>();
                var count = await provider.GetActiveCountAsync(stoppingToken);
                ProjectsMetrics.SetActiveProjectCount(count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                LogMetricUpdateFailed(ex);
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    [LoggerMessage(EventId = 1001, Level = LogLevel.Warning,
        Message = "Failed to update active project count metric")]
    private partial void LogMetricUpdateFailed(Exception exception);
}
