using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Dispatcher;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Responses;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;
using SmartSolutionsLab.BauDoku.Projects.Api.Mapping;
using SmartSolutionsLab.BauDoku.Projects.Application.Commands;
using SmartSolutionsLab.BauDoku.Projects.Application.Queries;
using SmartSolutionsLab.BauDoku.Projects.ReadModel;
using SmartSolutionsLab.BauDoku.Projects.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SmartSolutionsLab.BauDoku.Projects.Api.Endpoints;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects")
            .RequireAuthorization(AuthPolicies.RequireInspector)
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

        return app;
    }

    private static async Task<Created<CreatedResponse>> CreateProject(CreateProjectCommand command, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var projectId = await dispatcher.Send(command, cancellationToken);
        return TypedResults.Created($"/api/projects/{projectId.Value}", new CreatedResponse(projectId.Value));
    }

    private static async Task<Ok<PagedResult<ProjectListItemDto>>> ListProjects(string? search, int? page, int? pageSize, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new ListProjectsQuery(
            SearchTerm.FromNullable(search),
            PageNumber.FromNullable(page),
            PageSize.FromNullable(pageSize));

        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<Ok<ProjectDto>> GetProject(Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery(ProjectIdentifier.From(id));

        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<Ok<IReadOnlyList<ZoneDto>>> ListZones(Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery(ProjectIdentifier.From(id));
        var result = await dispatcher.Query(query, cancellationToken);

        return TypedResults.Ok(result.Zones);
    }

    private static async Task<NoContent> AddZone(Guid id, AddZoneRequest request, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        await dispatcher.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteProject(Guid id, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = new DeleteProjectCommand(ProjectIdentifier.From(id));
        await dispatcher.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
