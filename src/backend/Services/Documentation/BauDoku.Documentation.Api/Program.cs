using BauDoku.BuildingBlocks.Application;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.BuildingBlocks.Infrastructure.Serialization;
using BauDoku.Documentation.Api.Endpoints;
using BauDoku.Documentation.Infrastructure;
using BauDoku.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.DocumentationDb);

builder.AddServiceDefaults(health =>
{
    health.AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);
    health.AddNpgSql(connectionString, healthQuery: "SELECT PostGIS_Version()", name: "postgis", tags: ["ready"]);
});

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new ValueObjectJsonConverterFactory()));

builder.Services.AddOpenApi(options => options.AddSchemaTransformer<ValueObjectSchemaTransformer>());

builder.Services.AddBauDokuAuthentication(builder.Configuration, builder.Environment);

builder.Services.AddApplication(BauDoku.Documentation.Application.DependencyInjection.Assembly);
builder.Services.AddDocumentationInfrastructure(connectionString, builder.Configuration);

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
app.MapInstallationEndpoints();
app.MapPhotoEndpoints();
app.MapMeasurementEndpoints();
app.MapChunkedUploadEndpoints();

app.Run();

public partial class Program { }
