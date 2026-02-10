using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Infrastructure.Persistence;
using BauDoku.Sync.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Sync.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SyncDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SyncDbContext>());
        services.AddScoped<ISyncBatchRepository, SyncBatchRepository>();
        services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();

        var entityVersionStore = new ServiceDescriptor(
            typeof(EntityVersionStore),
            typeof(EntityVersionStore),
            ServiceLifetime.Scoped);
        services.Add(entityVersionStore);
        services.AddScoped<IEntityVersionStore>(sp => sp.GetRequiredService<EntityVersionStore>());
        services.AddScoped<IEntityVersionReadStore>(sp => sp.GetRequiredService<EntityVersionStore>());

        return services;
    }
}
