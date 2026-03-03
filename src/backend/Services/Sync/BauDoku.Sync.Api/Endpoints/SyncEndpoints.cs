using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Dispatcher;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;
using SmartSolutionsLab.BauDoku.Sync.Api.Mapping;
using SmartSolutionsLab.BauDoku.Sync.Application.Commands;
using SmartSolutionsLab.BauDoku.Sync.ReadModel;
using SmartSolutionsLab.BauDoku.Sync.Application.Queries;
using SmartSolutionsLab.BauDoku.Sync.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SmartSolutionsLab.BauDoku.Sync.Api.Endpoints;

public static class SyncEndpoints
{
    public static void MapSyncEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sync")
            .WithTags("Sync")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/batch", ProcessSyncBatch)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("ProcessSyncBatch")
            .WithSummary("Sync-Batch verarbeiten")
            .ProducesValidationProblem();

        group.MapGet("/changes", GetChangesSince)
            .WithName("GetChangesSince")
            .WithSummary("Änderungen seit einem Zeitpunkt abrufen");

        group.MapGet("/conflicts", GetConflicts)
            .WithName("GetConflicts")
            .WithSummary("Sync-Konflikte auflisten");

        group.MapPost("/conflicts/{id:guid}/resolve", ResolveConflict)
            .RequireAuthorization(AuthPolicies.RequireAdmin)
            .WithName("ResolveConflict")
            .WithSummary("Sync-Konflikt manuell auflösen")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<Ok<ProcessSyncBatchResult>> ProcessSyncBatch(
        ProcessSyncBatchCommand command, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await dispatcher.Send(command, cancellationToken));
    }

    private static async Task<Ok<ChangeSetResult>> GetChangesSince(
        string deviceId, DateTime? since, int? limit, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetChangesSinceQuery(DeviceIdentifier.From(deviceId), SyncLimit.FromNullable(limit), since);
        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<Ok<List<ConflictDto>>> GetConflicts(
        string? deviceId, string? status, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetConflictsQuery(DeviceIdentifier.FromNullable(deviceId), ConflictStatus.FromNullable(status));
        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<NoContent> ResolveConflict(
        Guid id, ResolveConflictRequest request, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        await dispatcher.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
