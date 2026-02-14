using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BauDoku.BuildingBlocks.Infrastructure.Auth;

public sealed class AuthAuditMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<AuthAuditMiddleware> logger;

    public AuthAuditMiddleware(RequestDelegate next, ILogger<AuthAuditMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode is 401 or 403)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var method = context.Request.Method;
            var path = context.Request.Path;

            logger.LogWarning(
                "Auth {StatusCode}: User={UserId} Method={Method} Path={Path}",
                context.Response.StatusCode, userId, method, path);
        }
    }
}

public static class AuthAuditMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthAuditLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthAuditMiddleware>();
    }
}
