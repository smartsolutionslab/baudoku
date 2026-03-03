using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Auth;
using BauDoku.Projects.Api.Mapping;
using BauDoku.Projects.Application.Commands;
using BauDoku.Projects.Application.Queries;
using BauDoku.Projects.ReadModel;
using BauDoku.Projects.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BauDoku.Projects.Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/", CreateProject)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("CreateProject")
            .WithSummary("Neues Projekt erstellen")
            .ProducesValidationProblem();

        group.MapGet("/", ListProjects)
            .WithName("ListProjects")
            .WithSummary("Projekte auflisten und suchen");

        group.MapGet("/{id:guid}", GetProject)
            .WithName("GetProject")
            .WithSummary("Projekt nach ID abrufen")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/zones", ListZones)
            .WithName("ListZones")
            .WithSummary("Zonen eines Projekts abrufen")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/zones", AddZone)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("AddZone")
            .WithSummary("Zone zu einem Projekt hinzufuegen")
            .ProducesValidationProblem();

        group.MapDelete("/{id:guid}", DeleteProject)
            .RequireAuthorization(AuthPolicies.RequireAdmin)
            .WithName("DeleteProject")
            .WithSummary("Projekt loeschen")
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<Created<CreatedResponse>> CreateProject(
        CreateProjectCommand command, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var projectId = await dispatcher.Send(command, cancellationToken);
        return TypedResults.Created($"/api/projects/{projectId.Value}", new CreatedResponse(projectId.Value));
    }

    private static async Task<Ok<PagedResult<ProjectListItemDto>>> ListProjects(
        string? search, int? page, int? pageSize, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new ListProjectsQuery(
            SearchTerm.FromNullable(search),
            PageNumber.FromNullable(page) ?? PageNumber.Default,
            PageSize.FromNullable(pageSize) ?? PageSize.Default);
        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<Ok<ProjectDto>> GetProject(
        Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery(ProjectIdentifier.From(id));
        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<Ok<IReadOnlyList<ZoneDto>>> ListZones(
        Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery(ProjectIdentifier.From(id));
        var result = await dispatcher.Query(query, cancellationToken);
        return TypedResults.Ok(result.Zones);
    }

    private static async Task<NoContent> AddZone(
        Guid id, AddZoneRequest request, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        await dispatcher.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteProject(
        Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = new DeleteProjectCommand(ProjectIdentifier.From(id));
        await dispatcher.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
