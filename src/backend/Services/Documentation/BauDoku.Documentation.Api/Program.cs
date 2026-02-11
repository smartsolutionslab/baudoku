using BauDoku.BuildingBlocks.Application;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Documentation.Api.Endpoints;
using BauDoku.Documentation.Infrastructure;
using BauDoku.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Services.AddBauDokuAuthentication(builder.Configuration);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapInstallationEndpoints();
app.MapPhotoEndpoints();
app.MapMeasurementEndpoints();

app.Run();
