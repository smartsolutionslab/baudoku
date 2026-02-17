using BauDoku.ApiGateway.Endpoints;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(health =>
{
    health.AddUrlGroup(new Uri("http://projects-api/alive"), name: "projects-api", tags: ["ready"]);
    health.AddUrlGroup(new Uri("http://documentation-api/alive"), name: "documentation-api", tags: ["ready"]);
    health.AddUrlGroup(new Uri("http://sync-api/alive"), name: "sync-api", tags: ["ready"]);
});

builder.Services.AddHttpClient();
builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetSection("Authentication:Keycloak"));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:8081",
                "http://localhost:19006",
                "exp://localhost:8081",
                "http://10.0.0.6:8081",
                "http://10.0.0.6:5000",
                "exp://10.0.0.6:8081")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Authorization");
    });
});

var app = builder.Build();

app.UseCors();
app.MapReverseProxy();
app.MapAuthEndpoints();
app.MapDefaultEndpoints();

app.Run();
