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

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var keycloak = configuration.GetSection("Authentication:Keycloak");
                options.Authority = keycloak["Authority"];
                options.RequireHttpsMetadata = !environment.IsDevelopment();

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
                    ValidateAudience = true,
                    ValidAudiences = keycloak.GetSection("Audiences").Get<string[]>()
                        ?? [keycloak["Audience"] ?? "baudoku-api"],
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
