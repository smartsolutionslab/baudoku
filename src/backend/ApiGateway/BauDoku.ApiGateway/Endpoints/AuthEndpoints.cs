using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace BauDoku.ApiGateway.Endpoints;

public static class AuthEndpoints
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
            logger.LogWarning("Keycloak authority not configured — logout skipped");
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
                logger.LogWarning("Keycloak token revocation returned {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to revoke token at Keycloak");
        }

        return TypedResults.Ok();
    }
}
