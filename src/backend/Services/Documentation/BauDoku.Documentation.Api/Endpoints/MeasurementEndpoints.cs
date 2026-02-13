using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Documentation.Application.Commands.RecordMeasurement;
using BauDoku.Documentation.Application.Commands.RemoveMeasurement;
using BauDoku.Documentation.Application.Queries.GetMeasurements;

namespace BauDoku.Documentation.Api.Endpoints;

public static class MeasurementEndpoints
{
    public static void MapMeasurementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation")
            .WithTags("Measurements")
            .RequireAuthorization();

        group.MapPost("/installations/{installationId:guid}/measurements", async (
            Guid installationId,
            RecordMeasurementRequest request,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new RecordMeasurementCommand(
                installationId,
                request.Type,
                request.Value,
                request.Unit,
                request.MinThreshold,
                request.MaxThreshold,
                request.Notes);

            var measurementId = await dispatcher.Send(command, ct);
            return Results.Created(
                $"/api/documentation/installations/{installationId}/measurements", new { id = measurementId });
        });

        group.MapGet("/installations/{installationId:guid}/measurements", async (
            Guid installationId,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetMeasurementsQuery(installationId);
            var result = await dispatcher.Query(query, ct);
            return Results.Ok(result);
        });

        group.MapDelete("/installations/{installationId:guid}/measurements/{measurementId:guid}", async (
            Guid measurementId,
            Guid installationId,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new RemoveMeasurementCommand(installationId, measurementId);
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        });
    }
}

public sealed record RecordMeasurementRequest(
    string Type,
    double Value,
    string Unit,
    double? MinThreshold,
    double? MaxThreshold,
    string? Notes);
