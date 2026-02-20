using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Documentation.Api.Mapping;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Domain;
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
        var group = app.MapGroup("/api/documentation/installations").WithTags("Installations");

        group.MapPost("/", async (DocumentInstallationCommand command, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var id = await dispatcher.Send(command, ct);
            return Results.Created($"/api/documentation/installations/{id}", new CreatedResponse(id));
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("DocumentInstallation")
        .WithSummary("Neue Installation dokumentieren")
        .Produces<CreatedResponse>(StatusCodes.Status201Created)
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
        .RequireAuthorization()
        .WithName("ListInstallations")
        .WithSummary("Installationen auflisten und filtern")
        .Produces<PagedResult<InstallationListItemDto>>(StatusCodes.Status200OK);

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
            var query = new GetInstallationsInRadiusQuery(latitude, longitude, radiusMeters, projectId, page ?? 1, pageSize ?? 20);
            return Results.Ok(await dispatcher.Query(query, ct));
        })
        .RequireAuthorization()
        .WithName("GetInstallationsNearby")
        .WithSummary("Installationen im Umkreis suchen")
        .Produces<PagedResult<NearbyInstallationDto>>(StatusCodes.Status200OK);

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
            var query = new GetInstallationsInBoundingBoxQuery(minLatitude, minLongitude, maxLatitude, maxLongitude, projectId, page ?? 1, pageSize ?? 20);
            return Results.Ok(await dispatcher.Query(query, ct));
        })
        .RequireAuthorization()
        .WithName("GetInstallationsInArea")
        .WithSummary("Installationen in einem Gebiet suchen")
        .Produces<PagedResult<InstallationListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var query = new GetInstallationQuery(id);
            var result = await dispatcher.Query(query, ct);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetInstallation")
        .WithSummary("Installation nach ID abrufen")
        .Produces<InstallationDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", async (Guid id, UpdateInstallationRequest request, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var command = request.ToCommand(id);

            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("UpdateInstallation")
        .WithSummary("Installation aktualisieren")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        group.MapPost("/{id:guid}/complete", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var command = new CompleteInstallationCommand(InstallationIdentifier.From(id));
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("CompleteInstallation")
        .WithSummary("Installation als abgeschlossen markieren")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var command = new DeleteInstallationCommand(InstallationIdentifier.From(id));
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireAdmin)
        .WithName("DeleteInstallation")
        .WithSummary("Installation loeschen")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
