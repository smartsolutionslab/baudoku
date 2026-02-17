using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BauDoku.BuildingBlocks.Infrastructure.Auth;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddBauDokuAuthentication(
        this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

        var keycloakSection = configuration.GetSection("Authentication:Keycloak");
        services.Configure<KeycloakOptions>(keycloakSection);

        var keycloak = keycloakSection.Get<KeycloakOptions>() ?? new KeycloakOptions();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keycloak.Authority;
                options.RequireHttpsMetadata = !environment.IsDevelopment();

                var validIssuers = new List<string> { keycloak.Authority };

                // In Development, also accept tokens issued via LAN IP (for mobile device testing)
                if (keycloak.AdditionalIssuers is { Length: > 0 })
                {
                    validIssuers.AddRange(keycloak.AdditionalIssuers);
                }

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudiences = keycloak.Audiences ?? [keycloak.Audience],
                    ValidateIssuer = true,
                    ValidIssuers = validIssuers,
                    RoleClaimType = ClaimTypes.Role,
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("BauDoku.Auth");
                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        logger.LogDebug("Token validated for user {UserId}", userId);
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("BauDoku.Auth");
                        logger.LogWarning(context.Exception, "Authentication failed");
                        return Task.CompletedTask;
                    },
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.RequireUser, policy =>
                policy.RequireRole("user", "admin"));

            options.AddPolicy(AuthPolicies.RequireAdmin, policy =>
                policy.RequireRole("admin"));
        });

        return services;
    }
}
