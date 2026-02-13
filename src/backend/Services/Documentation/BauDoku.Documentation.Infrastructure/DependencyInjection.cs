using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Infrastructure.Persistence;
using BauDoku.Documentation.Infrastructure.Persistence.Repositories;
using BauDoku.Documentation.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Documentation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentationInfrastructure(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DocumentationDbContext>(options =>
            options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DocumentationDbContext>());
        services.AddScoped<IInstallationRepository, InstallationRepository>();
        services.AddScoped<IInstallationReadRepository, InstallationReadRepository>();
        services.AddScoped<IPhotoReadRepository, PhotoReadRepository>();

        services.AddSingleton<IPhotoStorage>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var provider = config["PhotoStorage:Provider"];
            return provider?.Equals("Azure", StringComparison.OrdinalIgnoreCase) == true
                ? new AzureBlobPhotoStorage(config)
                : new LocalFilePhotoStorage(config);
        });

        services.AddSingleton<IChunkedUploadStorage, LocalChunkedUploadStorage>();
        services.AddHostedService<ChunkedUploadCleanupService>();

        return services;
    }
}
