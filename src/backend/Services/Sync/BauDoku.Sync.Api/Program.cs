using BauDoku.BuildingBlocks.Application;
using BauDoku.Sync.Api.Endpoints;
using BauDoku.Sync.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("SyncDb")
    ?? throw new InvalidOperationException("Connection string 'SyncDb' not found.");

builder.Services.AddApplication(BauDoku.Sync.Application.DependencyInjection.Assembly);
builder.Services.AddSyncInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");
app.MapSyncEndpoints();

app.Run();
