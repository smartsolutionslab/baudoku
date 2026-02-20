using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Infrastructure.Persistence;
using BauDoku.Documentation.Infrastructure.Persistence.Repositories;
using BauDoku.Documentation.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentationInfrastructure(this IServiceCollection services, string connectionString, IConfiguration configuration)
    {
        services.AddDbContext<DocumentationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.UseNetTopologySuite();
                npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DocumentationDbContext>());
        services.AddScoped<IInstallationRepository, InstallationRepository>();
        services.AddScoped<IInstallationReadRepository, InstallationReadRepository>();
        services.AddScoped<IPhotoReadRepository, PhotoReadRepository>();

        services.Configure<PhotoStorageOptions>(configuration.GetSection("PhotoStorage"));

        services.AddSingleton<IPhotoStorage>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PhotoStorageOptions>>();
            return options.Value.Provider.Equals("Azure", StringComparison.OrdinalIgnoreCase)
                ? new AzureBlobPhotoStorage(options)
                : new LocalFilePhotoStorage(options);
        });

        services.AddSingleton<IChunkedUploadStorage, LocalChunkedUploadStorage>();
        services.AddHostedService<ChunkedUploadCleanupService>();

        return services;
    }
}
