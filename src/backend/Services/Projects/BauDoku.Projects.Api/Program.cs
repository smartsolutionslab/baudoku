using BauDoku.BuildingBlocks.Application;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.BuildingBlocks.Infrastructure.Serialization;
using BauDoku.Projects.Api.Endpoints;
using BauDoku.Projects.Infrastructure;
using BauDoku.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ProjectsDb")
    ?? throw new InvalidOperationException("Connection string 'ProjectsDb' not found.");

builder.AddServiceDefaults(health =>
{
    health.AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);
});

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new ValueObjectJsonConverterFactory()));

builder.Services.AddOpenApi();

builder.Services.AddBauDokuAuthentication(builder.Configuration, builder.Environment);

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
app.UseAuthAuditLogging();

app.MapDefaultEndpoints();
app.MapProjectEndpoints();

app.Run();

public partial class Program { }
