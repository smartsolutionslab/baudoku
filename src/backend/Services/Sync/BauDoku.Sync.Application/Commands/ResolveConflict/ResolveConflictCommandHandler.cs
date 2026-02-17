using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Diagnostics;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Commands.ResolveConflict;

public sealed class ResolveConflictCommandHandler(ISyncBatchRepository syncBatches, IEntityVersionStore entityVersionStore, IUnitOfWork unitOfWork)
    : ICommandHandler<ResolveConflictCommand>
{
    public async Task Handle(ResolveConflictCommand command, CancellationToken cancellationToken = default)
    {
        var (conflictId, strategyName, mergedPayloadJson) = command;
        var conflictIdentifier = ConflictRecordIdentifier.From(conflictId);
        var batch = await syncBatches.GetByConflictIdAsync(conflictIdentifier, cancellationToken)
            ?? throw new KeyNotFoundException($"Batch fuer Konflikt {conflictId} nicht gefunden.");

        var strategy = ConflictResolutionStrategy.From(strategyName);
        var mergedPayload = mergedPayloadJson is not null ? DeltaPayload.From(mergedPayloadJson) : null;

        batch.ResolveConflict(conflictIdentifier, strategy, mergedPayload);

        var conflict = batch.Conflicts.First(c => c.Id == conflictIdentifier);

        if (strategy == ConflictResolutionStrategy.ClientWins || strategy == ConflictResolutionStrategy.ManualMerge)
        {
            var resolvedPayload = conflict.ResolvedPayload!;
            var (entityType, entityId) = conflict.EntityRef;
            var currentVersion = await entityVersionStore.GetCurrentVersionAsync(entityType, entityId, cancellationToken);
            var newVersion = currentVersion.Increment();

            await entityVersionStore.SetVersionAsync(
                entityType,
                entityId,
                newVersion,
                resolvedPayload.Value,
                batch.DeviceId,
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        SyncMetrics.ConflictsResolved.Add(1);
    }
}
