using BauDoku.ApiGateway.Endpoints;
using BauDoku.BuildingBlocks.Auth;
using BauDoku.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(health =>
{
    health.AddUrlGroup(new Uri("http://projects-api/alive"), name: "projects-api", tags: ["ready"]);
    health.AddUrlGroup(new Uri("http://documentation-api/alive"), name: "documentation-api", tags: ["ready"]);
    health.AddUrlGroup(new Uri("http://sync-api/alive"), name: "sync-api", tags: ["ready"]);
});

builder.Services.AddHttpClient();
builder.Services.AddKeycloakOptions(builder.Configuration);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var allowedOrigins = GetAllowedOrigins(builder);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Authorization");
    });
});

var app = builder.Build();

app.UseCors()
   .UseWebSockets();

app.MapReverseProxy();
app.MapAuthEndpoints()
   .MapSystemEndpoints();

app.MapDefaultEndpoints();

app.Run();

string[] GetAllowedOrigins(WebApplicationBuilder webApplicationBuilder)
{
    return webApplicationBuilder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? [];
}
