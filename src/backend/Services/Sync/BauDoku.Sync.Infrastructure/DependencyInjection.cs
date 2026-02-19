using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Domain;
using BauDoku.Sync.Infrastructure.BackgroundServices;
using BauDoku.Sync.Infrastructure.Persistence;
using BauDoku.Sync.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Sync.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SyncDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SyncDbContext>());
        services.AddScoped<ISyncBatchRepository, SyncBatchRepository>();
        services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();

        services.AddScoped<EntityVersionStore>();
        services.AddScoped<IEntityVersionStore>(sp => sp.GetRequiredService<EntityVersionStore>());
        services.AddScoped<IEntityVersionReadStore>(sp => sp.GetRequiredService<EntityVersionStore>());

        services.AddHostedService<SyncSchedulerService>();

        return services;
    }
}
