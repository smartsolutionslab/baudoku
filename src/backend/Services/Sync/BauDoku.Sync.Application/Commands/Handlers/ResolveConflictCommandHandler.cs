using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Diagnostics;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Commands.Handlers;

public sealed class ResolveConflictCommandHandler(ISyncBatchRepository syncBatches, IEntityVersionStore entityVersionStore, IUnitOfWork unitOfWork)
    : ICommandHandler<ResolveConflictCommand>
{
    public async Task Handle(ResolveConflictCommand command, CancellationToken cancellationToken = default)
    {
        var (conflictId, strategy, mergedPayload) = command;

        var batch = await syncBatches.GetByConflictIdAsync(conflictId, cancellationToken);

        batch.ResolveConflict(conflictId, strategy, mergedPayload);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);

        if (strategy == ConflictResolutionStrategy.ClientWins || strategy == ConflictResolutionStrategy.ManualMerge)
        {
            var entityRef = conflict.EntityRef;
            var resolvedPayload = conflict.ResolvedPayload!;
            var currentVersion = await entityVersionStore.GetCurrentVersionAsync(entityRef, cancellationToken);
            var newVersion = currentVersion.Increment();

            await entityVersionStore.SetVersionAsync(
                entityRef,
                newVersion,
                resolvedPayload.Value,
                batch.DeviceId,
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        SyncMetrics.ConflictsResolved.Add(1);
    }
}
