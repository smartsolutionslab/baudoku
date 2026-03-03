using System.Security.Claims;

namespace BauDoku.BuildingBlocks.Auth;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

    public static string[] GetRoles(this ClaimsPrincipal principal)
        => principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
}
