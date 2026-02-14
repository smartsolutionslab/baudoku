using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Projects.Application.Commands.AddZone;
using BauDoku.Projects.Application.Commands.CreateProject;
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
            return Results.Created($"/api/projects/{projectId}", new { id = projectId });
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("CreateProject")
        .Produces(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        group.MapGet("/", async (string? search, int? page, int? pageSize, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var query = new ListProjectsQuery(search, page ?? 1, pageSize ?? 20);
            var result = await dispatcher.Query(query, ct);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("ListProjects")
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var query = new GetProjectQuery(id);
            var result = await dispatcher.Query(query, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("GetProject")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/zones", async (Guid id, AddZoneRequest request, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var command = new AddZoneCommand(id, request.Name, request.Type, request.ParentZoneId);
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("AddZone")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem();
    }
}
