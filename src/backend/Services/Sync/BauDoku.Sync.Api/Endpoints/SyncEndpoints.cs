using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Sync.Api.Mapping;
using BauDoku.Sync.Application.Commands.ProcessSyncBatch;
using BauDoku.Sync.Application.Queries.Dtos;
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
        .WithSummary("Sync-Batch verarbeiten")
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
        .WithSummary("Aenderungen seit einem Zeitpunkt abrufen")
        .Produces<ChangeSetResult>(StatusCodes.Status200OK);

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
        .WithSummary("Sync-Konflikte auflisten")
        .Produces<List<ConflictDto>>(StatusCodes.Status200OK);

        group.MapPost("/conflicts/{id:guid}/resolve", async (
            Guid id,
            ResolveConflictRequest request,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = request.ToCommand(id);
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireAdmin)
        .WithName("ResolveConflict")
        .WithSummary("Sync-Konflikt manuell aufloesen")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();
    }
}
