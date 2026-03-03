using BauDoku.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Sync.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSyncApplication(this IServiceCollection services)
        => services.AddApplication(typeof(DependencyInjection).Assembly);
}
