using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.BuildingBlocks.Infrastructure.Auth;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddBauDokuAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var keycloak = configuration.GetSection("Authentication:Keycloak");
                options.Authority = keycloak["Authority"];
                options.RequireHttpsMetadata = false;

                var authority = keycloak["Authority"]!;
                var validIssuers = new List<string> { authority };

                // In Development, also accept tokens issued via LAN IP (for mobile device testing)
                var additionalIssuers = keycloak.GetSection("AdditionalIssuers").Get<string[]>();
                if (additionalIssuers is { Length: > 0 })
                {
                    validIssuers.AddRange(additionalIssuers);
                }

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuers = validIssuers,
                };
            });

        services.AddAuthorization();

        return services;
    }
}
