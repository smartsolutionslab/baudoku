using SmartSolutionsLab.BauDoku.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.BauDoku.Sync.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSyncApplication(this IServiceCollection services)
        => services.AddApplication(typeof(DependencyInjection).Assembly);
}
