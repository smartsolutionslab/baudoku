namespace BauDoku.ApiGateway.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/logout", async (
            LogoutRequest request,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<LogoutRequest> logger) =>
        {
            var authority = configuration["Authentication:Keycloak:Authority"];
            if (string.IsNullOrEmpty(authority))
            {
                logger.LogWarning("Keycloak authority not configured — logout skipped");
                return Results.Ok();
            }

            var revokeUrl = $"{authority}/protocol/openid-connect/revoke";

            var client = httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = configuration["Authentication:Keycloak:ClientId"] ?? "baudoku-app",
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

            return Results.Ok();
        })
        .WithTags("Auth")
        .WithName("Logout")
        .WithSummary("Revoke refresh token via Keycloak")
        .AllowAnonymous();
    }
}
