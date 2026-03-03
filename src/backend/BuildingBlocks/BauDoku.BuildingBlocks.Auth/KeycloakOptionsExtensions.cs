using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;

public static class KeycloakOptionsExtensions
{
    public static IServiceCollection AddKeycloakOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.SectionName));
        return services;
    }
}
