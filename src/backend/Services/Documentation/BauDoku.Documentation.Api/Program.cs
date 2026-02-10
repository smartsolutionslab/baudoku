using BauDoku.BuildingBlocks.Application;
using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Documentation.Api.Endpoints;
using BauDoku.Documentation.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DocumentationDb")
    ?? throw new InvalidOperationException("Connection string 'DocumentationDb' not found.");

builder.Services.AddApplication(BauDoku.Documentation.Application.DependencyInjection.Assembly);
builder.Services.AddDocumentationInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");
app.MapInstallationEndpoints();

app.Run();
