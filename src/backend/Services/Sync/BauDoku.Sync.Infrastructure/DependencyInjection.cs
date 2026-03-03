using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Persistence;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Persistence;
using SmartSolutionsLab.BauDoku.Sync.Application.Contracts;
using SmartSolutionsLab.BauDoku.Sync.ReadModel;
using SmartSolutionsLab.BauDoku.Sync.Domain;
using SmartSolutionsLab.BauDoku.Sync.Infrastructure.BackgroundServices;
using SmartSolutionsLab.BauDoku.Sync.Infrastructure.Persistence;
using SmartSolutionsLab.BauDoku.Sync.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.BauDoku.Sync.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SyncDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        services.AddDbContext<SyncReadDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        services.AddScoped<IUnitOfWork, UnitOfWork<SyncDbContext>>();
        services.AddScoped<ISyncBatchRepository, SyncBatchRepository>();
        services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();
        services.AddScoped<IEntityVersionStore, EntityVersionStore>();
        services.AddScoped<IEntityVersionReadStore, EntityVersionReadStore>();

        services.AddHostedService<SyncSchedulerService>();

        return services;
    }
}
