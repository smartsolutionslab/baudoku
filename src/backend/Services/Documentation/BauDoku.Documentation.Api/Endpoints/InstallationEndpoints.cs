using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Auth;
using BauDoku.Documentation.Api.Mapping;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Application.Queries;
using BauDoku.Documentation.ReadModel;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BauDoku.Documentation.Api.Endpoints;

public static class InstallationEndpoints
{
    public static void MapInstallationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation/installations")
            .WithTags("Installations")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/", DocumentInstallation)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("DocumentInstallation")
            .WithSummary("Neue Installation dokumentieren")
            .ProducesValidationProblem();

        group.MapGet("/", ListInstallations)
            .WithName("ListInstallations")
            .WithSummary("Installationen auflisten und filtern");

        group.MapGet("/nearby", GetInstallationsNearby)
            .WithName("GetInstallationsNearby")
            .WithSummary("Installationen im Umkreis suchen");

        group.MapGet("/in-area", GetInstallationsInArea)
            .WithName("GetInstallationsInArea")
            .WithSummary("Installationen in einem Gebiet suchen");

        group.MapGet("/{id:guid}", GetInstallation)
            .WithName("GetInstallation")
            .WithSummary("Installation nach ID abrufen")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateInstallation)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("UpdateInstallation")
            .WithSummary("Installation aktualisieren")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapPost("/{id:guid}/complete", CompleteInstallation)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("CompleteInstallation")
            .WithSummary("Installation als abgeschlossen markieren")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteInstallation)
            .RequireAuthorization(AuthPolicies.RequireAdmin)
            .WithName("DeleteInstallation")
            .WithSummary("Installation loeschen")
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Created<CreatedResponse>> DocumentInstallation(CreateInstallationRequest request, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var id = await dispatcher.Send(command, cancellationToken);

        return TypedResults.Created($"/api/documentation/installations/{id.Value}", new CreatedResponse(id.Value));
    }

    private static async Task<Ok<PagedResult<InstallationListItemDto>>> ListInstallations(
        Guid? projectId,
        Guid? zoneId,
        string? type,
        string? status,
        string? search,
        int? page,
        int? pageSize,
        IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var query = new ListInstallationsQuery(
            PageNumber.FromNullable(page),
            PageSize.FromNullable(pageSize),
            ProjectIdentifier.FromNullable(projectId),
            ZoneIdentifier.FromNullable(zoneId),
            InstallationType.FromNullable(type),
            InstallationStatus.FromNullable(status),
            SearchTerm.FromNullable(search));

        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<Ok<PagedResult<NearbyInstallationDto>>> GetInstallationsNearby(
        double latitude,
        double longitude,
        double radiusMeters,
        Guid? projectId,
        int? page,
        int? pageSize,
        IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var query = new GetInstallationsInRadiusQuery(
            new SearchRadius(
                Latitude.From(latitude),
                Longitude.From(longitude),
                RadiusMeters.From(radiusMeters)),
            PageNumber.FromNullable(page),
            PageSize.FromNullable(pageSize),
            ProjectIdentifier.FromNullable(projectId));

        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<Ok<PagedResult<InstallationListItemDto>>> GetInstallationsInArea(
        double minLatitude,
        double minLongitude,
        double maxLatitude,
        double maxLongitude,
        Guid? projectId,
        int? page,
        int? pageSize,
        IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var query = new GetInstallationsInBoundingBoxQuery(
            new BoundingBox(
                Latitude.From(minLatitude),
                Longitude.From(minLongitude),
                Latitude.From(maxLatitude),
                Longitude.From(maxLongitude)),
            PageNumber.FromNullable(page),
            PageSize.FromNullable(pageSize),
            ProjectIdentifier.FromNullable(projectId));

        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<Ok<InstallationDto>> GetInstallation(Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetInstallationQuery(InstallationIdentifier.From(id));

        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<NoContent> UpdateInstallation(Guid id, UpdateInstallationRequest request, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        await dispatcher.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> CompleteInstallation(Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = new CompleteInstallationCommand(InstallationIdentifier.From(id));
        await dispatcher.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteInstallation(Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = new DeleteInstallationCommand(InstallationIdentifier.From(id));
        await dispatcher.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
