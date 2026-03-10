using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace SmartSolutionsLab.BauDoku.ApiGateway.Endpoints;

public static partial class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/logout", Logout)
            .WithTags("Auth")
            .WithName("Logout")
            .WithSummary("Revoke refresh token via Keycloak")
            .AllowAnonymous();

        return app;
    }

    private static async Task<Ok> Logout(
        LogoutRequest request,
        IOptions<KeycloakOptions> keycloakOptions,
        IHttpClientFactory httpClientFactory,
        ILogger<LogoutRequest> logger)
    {
        var keycloak = keycloakOptions.Value;
        if (keycloak.Authority.HasNoValue())
        {
            LogKeycloakNotConfigured(logger);
            return TypedResults.Ok();
        }

        var revokeUrl = $"{keycloak.Authority}/protocol/openid-connect/revoke";

        var client = httpClientFactory.CreateClient();
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = keycloak.ClientId,
            ["token"] = request.RefreshToken,
            ["token_type_hint"] = "refresh_token"
        });

        try
        {
            var response = await client.PostAsync(revokeUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                LogTokenRevocationFailed(logger, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            LogTokenRevocationError(logger, ex);
        }

        return TypedResults.Ok();
    }

    [LoggerMessage(EventId = 7001, Level = LogLevel.Warning,
        Message = "Keycloak authority not configured — logout skipped")]
    private static partial void LogKeycloakNotConfigured(ILogger logger);

    [LoggerMessage(EventId = 7002, Level = LogLevel.Warning,
        Message = "Keycloak token revocation returned {StatusCode}")]
    private static partial void LogTokenRevocationFailed(ILogger logger, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(EventId = 7003, Level = LogLevel.Warning,
        Message = "Failed to revoke token at Keycloak")]
    private static partial void LogTokenRevocationError(ILogger logger, Exception exception);
}
