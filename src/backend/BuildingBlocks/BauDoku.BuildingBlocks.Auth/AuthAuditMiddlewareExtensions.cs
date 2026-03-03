using Microsoft.AspNetCore.Builder;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;

public static class AuthAuditMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthAuditLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthAuditMiddleware>();
    }
}
