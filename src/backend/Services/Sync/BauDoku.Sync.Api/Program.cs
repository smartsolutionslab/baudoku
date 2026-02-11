using BauDoku.BuildingBlocks.Application;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.ServiceDefaults;
using BauDoku.Sync.Api.Endpoints;
using BauDoku.Sync.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SyncDb")
    ?? throw new InvalidOperationException("Connection string 'SyncDb' not found.");

builder.AddServiceDefaults(health =>
{
    health.AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);

    var redisConnection = builder.Configuration.GetConnectionString("redis");
    if (!string.IsNullOrWhiteSpace(redisConnection))
    {
        health.AddRedis(redisConnection, name: "redis", tags: ["ready"]);
    }
});

builder.Services.AddOpenApi();

builder.Services.AddBauDokuAuthentication(builder.Configuration);

builder.Services.AddApplication(BauDoku.Sync.Application.DependencyInjection.Assembly);
builder.Services.AddSyncInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapSyncEndpoints();

app.Run();
