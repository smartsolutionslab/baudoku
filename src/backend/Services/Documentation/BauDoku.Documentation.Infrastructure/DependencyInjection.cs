using SmartSolutionsLab.BauDoku.Documentation.Application.Contracts;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;
using SmartSolutionsLab.BauDoku.Documentation.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Persistence;
using SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Persistence.Repositories;
using SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Projections;
using SmartSolutionsLab.BauDoku.Documentation.Infrastructure.ReadModel;
using SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Storage;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SmartSolutionsLab.BauDoku.Documentation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentationInfrastructure(this IServiceCollection services, string connectionString, IConfiguration configuration)
    {
        services.AddMarten(serviceProvider => {
            var options = new StoreOptions();
            MartenConfiguration.Configure(options, connectionString);
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            options.Listeners.Add(new MartenEventPublisher(scopeFactory));
            options.Projections.Add(new InstallationReadModelProjection(scopeFactory), ProjectionLifecycle.Async);
            return options;
        }).AddAsyncDaemon(DaemonMode.Solo);

        services.AddDbContext<ReadModelDbContext>(options => options.UseNpgsql(connectionString, npgsql => {
            npgsql.UseNetTopologySuite();
            npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }));

        services.AddScoped<IInstallationRepository, InstallationRepository>()
                .AddScoped<IInstallationReadRepository, InstallationReadRepository>()
                .AddScoped<IPhotoReadRepository, PhotoReadRepository>();

        services.Configure<PhotoStorageOptions>(configuration.GetSection(PhotoStorageOptions.SectionName));

        services.AddSingleton<IPhotoStorage>(sp => {
            var options = sp.GetRequiredService<IOptions<PhotoStorageOptions>>();
            return options.Value.IsAzure ? new AzureBlobPhotoStorage(options) : new LocalFilePhotoStorage(options);
        });

        services.AddSingleton<IChunkedUploadStorage, LocalChunkedUploadStorage>()
                .AddHostedService<ChunkedUploadCleanupService>();

        return services;
    }
}
