using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Auth;
using BauDoku.Documentation.Api.Mapping;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Application.Queries;
using BauDoku.Documentation.ReadModel;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BauDoku.Documentation.Api.Endpoints;

public static class MeasurementEndpoints
{
    public static void MapMeasurementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation")
            .WithTags("Measurements")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/installations/{installationId:guid}/measurements", RecordMeasurement)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("RecordMeasurement")
            .WithSummary("Messung zu einer Installation hinzufuegen")
            .ProducesValidationProblem();

        group.MapGet("/installations/{installationId:guid}/measurements", GetMeasurements)
            .WithName("GetMeasurements")
            .WithSummary("Messungen einer Installation auflisten")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/installations/{installationId:guid}/measurements/{measurementId:guid}", RemoveMeasurement)
            .RequireAuthorization(AuthPolicies.RequireAdmin)
            .WithName("RemoveMeasurement")
            .WithSummary("Messung von einer Installation entfernen")
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Created<CreatedResponse>> RecordMeasurement(Guid installationId, RecordMeasurementRequest request, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = request.ToCommand(installationId);
        var measurementId = await dispatcher.Send(command, cancellationToken);

        return TypedResults.Created($"/api/documentation/installations/{installationId}/measurements", new CreatedResponse(measurementId.Value));
    }

    private static async Task<Ok<IReadOnlyList<MeasurementDto>>> GetMeasurements(Guid installationId, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetMeasurementsQuery(InstallationIdentifier.From(installationId));

        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<NoContent> RemoveMeasurement(Guid measurementId, Guid installationId, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = new RemoveMeasurementCommand(
            InstallationIdentifier.From(installationId),
            MeasurementIdentifier.From(measurementId));
        await dispatcher.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
