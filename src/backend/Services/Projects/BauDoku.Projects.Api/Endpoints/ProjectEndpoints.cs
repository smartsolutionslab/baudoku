using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Projects.Application.Commands.AddZone;
using BauDoku.Projects.Application.Commands.CreateProject;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Application.Queries.GetProject;
using BauDoku.Projects.Application.Queries.ListProjects;

namespace BauDoku.Projects.Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        group.MapPost("/", async (CreateProjectCommand command, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var projectId = await dispatcher.Send<Guid>(command, ct);
            return Results.Created($"/api/projects/{projectId}", new CreatedResponse(projectId));
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("CreateProject")
        .WithSummary("Neues Projekt erstellen")
        .Produces<CreatedResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        group.MapGet("/", async (string? search, int? page, int? pageSize, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var query = new ListProjectsQuery(search, page ?? 1, pageSize ?? 20);
            var result = await dispatcher.Query(query, ct);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("ListProjects")
        .WithSummary("Projekte auflisten und suchen")
        .Produces<PagedResult<ProjectListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var query = new GetProjectQuery(id);
            var result = await dispatcher.Query(query, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("GetProject")
        .WithSummary("Projekt nach ID abrufen")
        .Produces<ProjectDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/zones", async (Guid id, AddZoneRequest request, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var command = new AddZoneCommand(id, request.Name, request.Type, request.ParentZoneId);
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("AddZone")
        .WithSummary("Zone zu einem Projekt hinzufuegen")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem();
    }
}
