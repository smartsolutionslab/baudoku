using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Documentation.Api.Mapping;
using BauDoku.Documentation.Application.Commands.RemoveMeasurement;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Application.Queries.GetMeasurements;

namespace BauDoku.Documentation.Api.Endpoints;

public static class MeasurementEndpoints
{
    public static void MapMeasurementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation").WithTags("Measurements");

        group.MapPost("/installations/{installationId:guid}/measurements", async (
            Guid installationId,
            RecordMeasurementRequest request,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = request.ToCommand(installationId);

            var measurementId = await dispatcher.Send(command, ct);
            return Results.Created(
                $"/api/documentation/installations/{installationId}/measurements", new CreatedResponse(measurementId));
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("RecordMeasurement")
        .WithSummary("Messung zu einer Installation hinzufuegen")
        .Produces<CreatedResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        group.MapGet("/installations/{installationId:guid}/measurements", async (
            Guid installationId,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetMeasurementsQuery(installationId);
            var result = await dispatcher.Query(query, ct);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetMeasurements")
        .WithSummary("Messungen einer Installation auflisten")
        .Produces<IReadOnlyList<MeasurementDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/installations/{installationId:guid}/measurements/{measurementId:guid}", async (
            Guid measurementId,
            Guid installationId,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new RemoveMeasurementCommand(InstallationIdentifier.From(installationId), MeasurementIdentifier.From(measurementId));
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireAdmin)
        .WithName("RemoveMeasurement")
        .WithSummary("Messung von einer Installation entfernen")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
