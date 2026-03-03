using BauDoku.BuildingBlocks.Auth;
using BauDoku.ServiceDefaults;
using BauDoku.Sync.Api.Endpoints;
using BauDoku.Sync.Application;
using BauDoku.Sync.Infrastructure;
using BauDoku.Sync.Infrastructure.BackgroundServices;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.SyncDb);

builder.AddServiceDefaults(health =>
{
    health.AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);
});

builder.AddBauDokuApiDefaults();

builder.Services.Configure<SyncOptions>(builder.Configuration.GetSection("Sync"))
                .AddSyncApplication()
                .AddSyncInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication()
    .UseAuthorization()
    .UseAuthAuditLogging();

app.MapDefaultEndpoints()
    .MapSyncEndpoints();

app.Run();

public partial class Program { }
