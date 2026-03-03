using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SmartSolutionsLab.BauDoku.ApiGateway.Endpoints;

public static class SystemEndpoints
{
    private static readonly string[] ServiceUrls =
    [
        "http://projects-api/version",
        "http://documentation-api/version",
        "http://sync-api/version"
    ];

    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/system/info", GetSystemInfo)
            .WithTags("System")
            .WithName("GetSystemInfo")
            .WithSummary("Aggregated version info for all services")
            .AllowAnonymous();

        return app;

    }

    private static async Task<Ok<SystemInfoResponse>> GetSystemInfo(IHttpClientFactory httpClientFactory)
    {
        var assembly = Assembly.GetEntryAssembly();
        var gatewayVersion = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "dev";
        var gatewayName = assembly?.GetName().Name ?? "unknown";

        var client = httpClientFactory.CreateClient();
        var services = await Task.WhenAll(ServiceUrls.Select(url => FetchVersionAsync(client, url)));

        return TypedResults.Ok(new SystemInfoResponse(new ServiceInfo(gatewayName, gatewayVersion, "ok"), services));
    }

    private static async Task<ServiceInfo> FetchVersionAsync(HttpClient client, string url)
    {
        try
        {
            var response = await client.GetFromJsonAsync<ServiceVersion>(url);
            return new ServiceInfo(response?.Service ?? "unknown", response?.Version ?? "unknown", "ok");
        }
        catch
        {
            var serviceName = url.Replace("http://", "").Replace("/version", "");
            return new ServiceInfo(serviceName, "unknown", "unreachable");
        }
    }

    private sealed record ServiceVersion(string Service, string Version);
    internal sealed record SystemInfoResponse(ServiceInfo Gateway, ServiceInfo[] Services);
    internal sealed record ServiceInfo(string Service, string Version, string Status);
}
