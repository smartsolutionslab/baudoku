using BauDoku.BuildingBlocks.Application;
using BauDoku.BuildingBlocks.Auth;
using BauDoku.Projects.Api.Endpoints;
using BauDoku.Projects.Infrastructure;
using BauDoku.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.ProjectsDb);

builder.AddServiceDefaults(health =>
{
    health.AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);
});

builder.AddBauDokuApiDefaults();

builder.Services.AddApplication(BauDoku.Projects.Application.DependencyInjection.Assembly)
                .AddProjectsInfrastructure(connectionString);

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
app.MapProjectEndpoints();

app.Run();

public partial class Program { }
