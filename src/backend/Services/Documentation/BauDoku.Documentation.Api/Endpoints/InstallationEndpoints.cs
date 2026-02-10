using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Documentation.Application.Commands.DocumentInstallation;
using BauDoku.Documentation.Application.Queries.GetInstallation;
using BauDoku.Documentation.Application.Queries.ListInstallations;

namespace BauDoku.Documentation.Api.Endpoints;

public static class InstallationEndpoints
{
    public static void MapInstallationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation/installations")
            .WithTags("Installations");

        group.MapPost("/", async (
            DocumentInstallationCommand command,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var id = await dispatcher.Send(command, ct);
            return Results.Created($"/api/documentation/installations/{id}", new { id });
        });

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
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetInstallationQuery(id);
            var result = await dispatcher.Query(query, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });
    }
}
