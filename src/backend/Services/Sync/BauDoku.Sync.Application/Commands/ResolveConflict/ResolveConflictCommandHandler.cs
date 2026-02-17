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
        var conflictId = ConflictRecordIdentifier.From(command.ConflictId);
        var batch = await syncBatches.GetByConflictIdAsync(conflictId, cancellationToken) ?? throw new KeyNotFoundException($"Batch fuer Konflikt {command.ConflictId} nicht gefunden.");

        var strategy = ConflictResolutionStrategy.From(command.Strategy);
        var mergedPayload = command.MergedPayload is not null ? DeltaPayload.From(command.MergedPayload) : null;

        batch.ResolveConflict(conflictId, strategy, mergedPayload);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);

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
