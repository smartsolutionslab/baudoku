using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;

public sealed class AuthAuditMiddleware(RequestDelegate next, ILogger<AuthAuditMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode is 401 or 403)
        {
            var userId = context.User.GetUserId();
            var method = context.Request.Method;
            var path = context.Request.Path;

            logger.LogWarning("Auth {StatusCode}: User={UserId} Method={Method} Path={Path}", context.Response.StatusCode, userId, method, path);
        }
    }
}
