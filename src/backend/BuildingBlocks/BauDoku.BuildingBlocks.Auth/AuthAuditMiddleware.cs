using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;

public sealed partial class AuthAuditMiddleware(RequestDelegate next, ILogger<AuthAuditMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode is 401 or 403)
        {
            var userId = context.User.GetUserId();
            var method = context.Request.Method;
            var path = context.Request.Path;

            LogAuthFailure(context.Response.StatusCode, userId, method, path.Value ?? "/");
        }
    }

    [LoggerMessage(EventId = 9030, Level = LogLevel.Warning,
        Message = "Auth {StatusCode}: User={UserId} Method={Method} Path={Path}")]
    private partial void LogAuthFailure(int statusCode, string userId, string method, string path);
}
