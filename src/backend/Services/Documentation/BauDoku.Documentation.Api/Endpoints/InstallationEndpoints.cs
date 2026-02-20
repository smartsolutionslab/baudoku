using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Documentation.Api.Mapping;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Application.Queries;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Api.Endpoints;

public static class InstallationEndpoints
{
    public static void MapInstallationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation/installations").WithTags("Installations");

        group.MapPost("/", async (DocumentInstallationCommand command, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var id = await dispatcher.Send<InstallationIdentifier>(command, cancellationToken);
            return Results.Created($"/api/documentation/installations/{id.Value}", new CreatedResponse(id.Value));
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
                ProjectIdentifier.FromNullable(projectId),
                ZoneIdentifier.FromNullable(zoneId),
                InstallationType.FromNullable(type),
                InstallationStatus.FromNullable(status),
                SearchTerm.FromNullable(search),
                page.HasValue ? PageNumber.From(page.Value) : null,
                pageSize.HasValue ? PageSize.From(pageSize.Value) : null);
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
            var query = new GetInstallationsInRadiusQuery(
                new SearchRadius(latitude, longitude, radiusMeters),
                ProjectIdentifier.FromNullable(projectId),
                page.HasValue ? PageNumber.From(page.Value) : null,
                pageSize.HasValue ? PageSize.From(pageSize.Value) : null);
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
            var query = new GetInstallationsInBoundingBoxQuery(
                new BoundingBox(minLatitude, minLongitude, maxLatitude, maxLongitude),
                ProjectIdentifier.FromNullable(projectId),
                page.HasValue ? PageNumber.From(page.Value) : null,
                pageSize.HasValue ? PageSize.From(pageSize.Value) : null);
            return Results.Ok(await dispatcher.Query(query, ct));
        })
        .RequireAuthorization()
        .WithName("GetInstallationsInArea")
        .WithSummary("Installationen in einem Gebiet suchen")
        .Produces<PagedResult<InstallationListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var query = new GetInstallationQuery(InstallationIdentifier.From(id));
            var result = await dispatcher.Query(query, cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetInstallation")
        .WithSummary("Installation nach ID abrufen")
        .Produces<InstallationDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", async (Guid id, UpdateInstallationRequest request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var command = request.ToCommand(id);

            await dispatcher.Send(command, cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("UpdateInstallation")
        .WithSummary("Installation aktualisieren")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        group.MapPost("/{id:guid}/complete", async (Guid id, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var command = new CompleteInstallationCommand(InstallationIdentifier.From(id));
            await dispatcher.Send(command, cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("CompleteInstallation")
        .WithSummary("Installation als abgeschlossen markieren")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var command = new DeleteInstallationCommand(InstallationIdentifier.From(id));
            await dispatcher.Send(command, cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireAdmin)
        .WithName("DeleteInstallation")
        .WithSummary("Installation loeschen")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
