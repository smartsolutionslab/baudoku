using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BauDoku.Projects.Infrastructure.Services;

public sealed class ActiveProjectCountService(IServiceScopeFactory scopeFactory, ILogger<ActiveProjectCountService> logger)
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
                logger.LogWarning(ex, "Failed to update active project count metric");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
