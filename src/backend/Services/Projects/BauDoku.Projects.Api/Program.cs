using BauDoku.BuildingBlocks.Application;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Projects.Api.Endpoints;
using BauDoku.Projects.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

builder.Services.AddBauDokuAuthentication(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("ProjectsDb")
    ?? throw new InvalidOperationException("Connection string 'ProjectsDb' not found.");

builder.Services.AddApplication(BauDoku.Projects.Application.DependencyInjection.Assembly);
builder.Services.AddProjectsInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapProjectEndpoints();

app.Run();
