using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Projects.Api.Mapping;
using BauDoku.Projects.Application.Commands;
using BauDoku.Projects.Application.Queries;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        group.MapPost("/", async (CreateProjectCommand command, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var projectId = await dispatcher.Send<ProjectIdentifier>(command, cancellationToken);
            return Results.Created($"/api/projects/{projectId.Value}", new CreatedResponse(projectId.Value));
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("CreateProject")
        .WithSummary("Neues Projekt erstellen")
        .Produces<CreatedResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        group.MapGet("/", async (string? search, int? page, int? pageSize, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var query = new ListProjectsQuery(
                SearchTerm.FromNullable(search),
                PageNumber.FromNullable(page) ?? PageNumber.Default,
                PageSize.FromNullable(pageSize) ?? PageSize.Default);
            var result = await dispatcher.Query(query, cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("ListProjects")
        .WithSummary("Projekte auflisten und suchen")
        .Produces<PagedResult<ProjectListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var query = new GetProjectQuery(ProjectIdentifier.From(id));
            var result = await dispatcher.Query(query, cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetProject")
        .WithSummary("Projekt nach ID abrufen")
        .Produces<ProjectDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/zones", async (Guid id, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var query = new GetProjectQuery(ProjectIdentifier.From(id));
            var result = await dispatcher.Query(query, cancellationToken);
            return Results.Ok(result.Zones);
        })
        .RequireAuthorization()
        .WithName("ListZones")
        .WithSummary("Zonen eines Projekts abrufen")
        .Produces<IReadOnlyList<ZoneDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/zones", async (Guid id, AddZoneRequest request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var command = request.ToCommand(id);
            await dispatcher.Send(command, cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("AddZone")
        .WithSummary("Zone zu einem Projekt hinzufuegen")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem();

        group.MapDelete("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var command = new DeleteProjectCommand(ProjectIdentifier.From(id));
            await dispatcher.Send(command, cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireAdmin)
        .WithName("DeleteProject")
        .WithSummary("Projekt loeschen")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
