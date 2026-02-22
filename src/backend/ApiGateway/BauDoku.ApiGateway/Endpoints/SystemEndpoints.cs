using System.Reflection;

namespace BauDoku.ApiGateway.Endpoints;

public static class SystemEndpoints
{
    private static readonly string[] ServiceUrls =
    [
        "http://projects-api/version",
        "http://documentation-api/version",
        "http://sync-api/version"
    ];

    public static void MapSystemEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/system/info", async (IHttpClientFactory httpClientFactory) =>
        {
            var assembly = Assembly.GetEntryAssembly();
            var gatewayVersion = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "dev";
            var gatewayName = assembly?.GetName().Name ?? "unknown";

            var client = httpClientFactory.CreateClient();
            var services = await Task.WhenAll(ServiceUrls.Select(url => FetchVersionAsync(client, url)));

            return new
            {
                gateway = new { service = gatewayName, version = gatewayVersion },
                services
            };
        })
        .WithTags("System")
        .WithName("GetSystemInfo")
        .WithSummary("Aggregated version info for all services")
        .AllowAnonymous();
    }

    private static async Task<object> FetchVersionAsync(HttpClient client, string url)
    {
        try
        {
            var response = await client.GetFromJsonAsync<ServiceVersion>(url);
            return new { service = response?.Service ?? "unknown", version = response?.Version ?? "unknown", status = "ok" };
        }
        catch
        {
            var serviceName = url.Replace("http://", "").Replace("/version", "");
            return new { service = serviceName, version = "unknown", status = "unreachable" };
        }
    }

    private sealed record ServiceVersion(string Service, string Version);
}
