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
    private const string AuthLogCategory = "BauDoku.Auth";

    public static IServiceCollection AddBauDokuAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

        var keycloakOptions = GetKeycloakOptions(services, configuration);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtBearerOptions =>
        {
            jwtBearerOptions.Authority = keycloakOptions.Authority;
            jwtBearerOptions.RequireHttpsMetadata = !environment.IsDevelopment();

            List<string> validIssuers = [keycloakOptions.Authority];

            // In Development, also accept tokens issued via LAN IP (for mobile device testing)
            if (keycloakOptions.AdditionalIssuers is { Length: > 0 })
            {
                validIssuers.AddRange(keycloakOptions.AdditionalIssuers);
            }

            jwtBearerOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudiences = keycloakOptions.Audiences ?? [keycloakOptions.Audience],
                ValidateIssuer = true,
                ValidIssuers = validIssuers,
                RoleClaimType = ClaimTypes.Role,
            };

            jwtBearerOptions.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(AuthLogCategory);
                    var principal = context.Principal;
                    var userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                    var roles = principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray() ?? [];
                    var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
                    logger.LogInformation("User authenticated: {UserId}, Roles: [{Roles}], IP: {IpAddress}", userId, string.Join(", ", roles), ipAddress);
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(AuthLogCategory);
                    logger.LogWarning(context.Exception, "Authentication failed");
                    return Task.CompletedTask;
                },
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.RequireUser, policy => policy.RequireRole("user", "admin"));
            options.AddPolicy(AuthPolicies.RequireAdmin, policy => policy.RequireRole("admin"));
        });

        return services;
    }

    private static KeycloakOptions GetKeycloakOptions(IServiceCollection services, IConfiguration configuration)
    {
        var keycloakSection = configuration.GetSection("Authentication:Keycloak");
        services.Configure<KeycloakOptions>(keycloakSection);

        var keycloak = keycloakSection.Get<KeycloakOptions>() ?? new KeycloakOptions();
        return keycloak;
    }
}
