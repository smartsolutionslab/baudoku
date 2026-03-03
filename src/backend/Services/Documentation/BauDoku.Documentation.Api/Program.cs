using SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;
using SmartSolutionsLab.BauDoku.Documentation.Api.Endpoints;
using SmartSolutionsLab.BauDoku.Documentation.Application;
using SmartSolutionsLab.BauDoku.Documentation.Infrastructure;
using SmartSolutionsLab.BauDoku.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.DocumentationDb);

builder.AddServiceDefaults(health =>
{
    health.AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);
    health.AddNpgSql(connectionString, healthQuery: "SELECT PostGIS_Version()", name: "postgis", tags: ["ready"]);
});

builder.AddBauDokuApiDefaults();

builder.Services.AddDocumentationApplication()
                .AddDocumentationInfrastructure(connectionString, builder.Configuration);

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
    .MapInstallationEndpoints()
    .MapPhotoEndpoints()
    .MapMeasurementEndpoints()
    .MapChunkedUploadEndpoints();

app.Run();

public partial class Program { }
