using SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;
using SmartSolutionsLab.BauDoku.Projects.Api.Endpoints;
using SmartSolutionsLab.BauDoku.Projects.Application;
using SmartSolutionsLab.BauDoku.Projects.Infrastructure;
using SmartSolutionsLab.BauDoku.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.ProjectsDb);

builder.AddServiceDefaults(health =>
{
    health.AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);
});

builder.AddBauDokuApiDefaults();

builder.Services.AddProjectsApplication()
                .AddProjectsInfrastructure(connectionString);

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
    .MapProjectEndpoints();

app.Run();

public partial class Program { }
