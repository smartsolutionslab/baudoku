using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace BauDoku.BuildingBlocks.Infrastructure.Auth;

public sealed class KeycloakClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity is null || !identity.IsAuthenticated)
            return Task.FromResult(principal);

        var realmAccessClaim = principal.FindFirst("realm_access");
        if (realmAccessClaim is null)
            return Task.FromResult(principal);

        using var doc = JsonDocument.Parse(realmAccessClaim.Value);
        if (!doc.RootElement.TryGetProperty("roles", out var rolesElement))
            return Task.FromResult(principal);

        foreach (var role in rolesElement.EnumerateArray())
        {
            var roleValue = role.GetString();
            if (roleValue is not null && !principal.IsInRole(roleValue))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
            }
        }

        return Task.FromResult(principal);
    }
}
