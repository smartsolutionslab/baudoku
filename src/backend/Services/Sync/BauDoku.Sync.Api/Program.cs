using BauDoku.BuildingBlocks.Application;
using BauDoku.BuildingBlocks.Auth;
using BauDoku.ServiceDefaults;
using BauDoku.Sync.Api.Endpoints;
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
                .AddApplication(BauDoku.Sync.Application.DependencyInjection.Assembly)
                .AddSyncInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseAuthAuditLogging();

app.MapDefaultEndpoints();
app.MapSyncEndpoints();

app.Run();

public partial class Program { }
