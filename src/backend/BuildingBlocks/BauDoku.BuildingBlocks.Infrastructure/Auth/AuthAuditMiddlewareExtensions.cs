using Microsoft.AspNetCore.Builder;

namespace BauDoku.BuildingBlocks.Infrastructure.Auth;

public static class AuthAuditMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthAuditLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthAuditMiddleware>();
    }
}
