using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace BauDoku.BuildingBlocks.Infrastructure.Auth;

public sealed class KeycloakClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity is null || !identity.IsAuthenticated) return Task.FromResult(principal);

        var realmAccessClaim = principal.FindFirst("realm_access");
        if (realmAccessClaim is null) return Task.FromResult(principal);

        using var doc = JsonDocument.Parse(realmAccessClaim.Value);
        if (!doc.RootElement.TryGetProperty("roles", out var rolesElement)) return Task.FromResult(principal);

        var roles = rolesElement.EnumerateArray()
            .Select(role => role.GetString()).OfType<string>()
            .Where(roleValue => !principal.IsInRole(roleValue));

        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        identity.AddClaims(claims);

        return Task.FromResult(principal);
    }
}
