using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;

public static partial class AuthenticationExtensions
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
                    var logger = CreateLogger(context.HttpContext);
                    var principal = context.Principal!;
                    var userId = principal.GetUserId();
                    var roles = principal.GetRoles();
                    var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
                    LogUserAuthenticated(logger, userId, string.Join(", ", roles), ipAddress);
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var logger = CreateLogger(context.HttpContext);
                    LogAuthenticationFailed(logger, context.Exception);

                    return Task.CompletedTask;
                },
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.RequireInspector, policy => policy.RequireRole(AuthRoles.Inspector, AuthRoles.User, AuthRoles.Admin));
            options.AddPolicy(AuthPolicies.RequireUser, policy => policy.RequireRole(AuthRoles.User, AuthRoles.Admin));
            options.AddPolicy(AuthPolicies.RequireAdmin, policy => policy.RequireRole(AuthRoles.Admin));
        });

        return services;
    }

    private static ILogger CreateLogger(HttpContext httpContext)
    {
        var serviceProvider = httpContext.RequestServices;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(AuthLogCategory);
        return logger;
    }

    private static KeycloakOptions GetKeycloakOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddKeycloakOptions(configuration);

        var keycloak = configuration.GetSection(KeycloakOptions.SectionName).Get<KeycloakOptions>() ?? new KeycloakOptions();
        return keycloak;
    }

    [LoggerMessage(EventId = 9040, Level = LogLevel.Information,
        Message = "User authenticated: {UserId}, Roles: [{Roles}], IP: {IpAddress}")]
    private static partial void LogUserAuthenticated(ILogger logger, string userId, string roles, System.Net.IPAddress? ipAddress);

    [LoggerMessage(EventId = 9041, Level = LogLevel.Warning,
        Message = "Authentication failed")]
    private static partial void LogAuthenticationFailed(ILogger logger, Exception exception);
}
