using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Documentation.Application.Commands.DocumentInstallation;
using BauDoku.Documentation.Application.Commands.UpdateInstallation;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Application.Queries.GetInstallation;
using BauDoku.Documentation.Application.Queries.GetInstallationsInBoundingBox;
using BauDoku.Documentation.Application.Queries.GetInstallationsInRadius;
using BauDoku.Documentation.Application.Queries.ListInstallations;

namespace BauDoku.Documentation.Api.Endpoints;

public static class InstallationEndpoints
{
    public static void MapInstallationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation/installations")
            .WithTags("Installations")
            .RequireAuthorization();

        group.MapPost("/", async (
            DocumentInstallationCommand command,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var id = await dispatcher.Send(command, ct);
            return Results.Created($"/api/documentation/installations/{id}", new { id });
        })
        .WithName("DocumentInstallation")
        .WithSummary("Neue Installation dokumentieren")
        .Produces<object>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        group.MapGet("/", async (
            Guid? projectId,
            Guid? zoneId,
            string? type,
            string? status,
            string? search,
            int? page,
            int? pageSize,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new ListInstallationsQuery(
                projectId, zoneId, type, status, search,
                page ?? 1, pageSize ?? 20);
            var result = await dispatcher.Query(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListInstallations")
        .WithSummary("Installationen auflisten und filtern")
        .Produces<object>(StatusCodes.Status200OK);

        group.MapGet("/nearby", async (
            double latitude,
            double longitude,
            double radiusMeters,
            Guid? projectId,
            int? page,
            int? pageSize,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetInstallationsInRadiusQuery(
                latitude, longitude, radiusMeters, projectId,
                page ?? 1, pageSize ?? 20);
            return Results.Ok(await dispatcher.Query(query, ct));
        })
        .WithName("GetInstallationsNearby")
        .WithSummary("Installationen im Umkreis suchen")
        .Produces<object>(StatusCodes.Status200OK);

        group.MapGet("/in-area", async (
            double minLatitude,
            double minLongitude,
            double maxLatitude,
            double maxLongitude,
            Guid? projectId,
            int? page,
            int? pageSize,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetInstallationsInBoundingBoxQuery(
                minLatitude, minLongitude, maxLatitude, maxLongitude,
                projectId, page ?? 1, pageSize ?? 20);
            return Results.Ok(await dispatcher.Query(query, ct));
        })
        .WithName("GetInstallationsInArea")
        .WithSummary("Installationen in einem Gebiet suchen")
        .Produces<object>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (
            Guid id,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetInstallationQuery(id);
            var result = await dispatcher.Query(query, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetInstallation")
        .WithSummary("Installation nach ID abrufen")
        .Produces<InstallationDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateInstallationRequest request,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new UpdateInstallationCommand(
                id,
                request.Latitude,
                request.Longitude,
                request.Altitude,
                request.HorizontalAccuracy,
                request.GpsSource,
                request.CorrectionService,
                request.RtkFixStatus,
                request.SatelliteCount,
                request.Hdop,
                request.CorrectionAge,
                request.Description,
                request.CableType,
                request.CrossSection,
                request.CableColor,
                request.ConductorCount,
                request.DepthMm);

            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .WithName("UpdateInstallation")
        .WithSummary("Installation aktualisieren")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();
    }
}

public sealed record UpdateInstallationRequest(
    double? Latitude,
    double? Longitude,
    double? Altitude,
    double? HorizontalAccuracy,
    string? GpsSource,
    string? CorrectionService,
    string? RtkFixStatus,
    int? SatelliteCount,
    double? Hdop,
    double? CorrectionAge,
    string? Description,
    string? CableType,
    decimal? CrossSection,
    string? CableColor,
    int? ConductorCount,
    int? DepthMm);
