using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Sync.Application.Commands.ProcessSyncBatch;
using BauDoku.Sync.Application.Commands.ResolveConflict;
using BauDoku.Sync.Application.Queries.GetChangesSince;
using BauDoku.Sync.Application.Queries.GetConflicts;

namespace BauDoku.Sync.Api.Endpoints;

public static class SyncEndpoints
{
    public static void MapSyncEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sync").WithTags("Sync");

        group.MapPost("/batch", async (
            ProcessSyncBatchCommand command,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var result = await dispatcher.Send<ProcessSyncBatchResult>(command, ct);
            return Results.Ok(result);
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("ProcessSyncBatch")
        .Produces<ProcessSyncBatchResult>(StatusCodes.Status200OK)
        .ProducesValidationProblem();

        group.MapGet("/changes", async (
            string deviceId,
            DateTime? since,
            int? limit,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetChangesSinceQuery(deviceId, since, limit);
            var result = await dispatcher.Query(query, ct);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetChangesSince")
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/conflicts", async (
            string? deviceId,
            string? status,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetConflictsQuery(deviceId, status);
            var result = await dispatcher.Query(query, ct);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("GetConflicts")
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/conflicts/{id:guid}/resolve", async (
            Guid id,
            ResolveConflictRequest request,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new ResolveConflictCommand(id, request.Strategy, request.MergedPayload);
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireAdmin)
        .WithName("ResolveConflict")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem();
    }
}
