using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Diagnostics;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Commands.ResolveConflict;

public sealed class ResolveConflictCommandHandler : ICommandHandler<ResolveConflictCommand>
{
    private readonly ISyncBatchRepository syncBatchRepository;
    private readonly IEntityVersionStore entityVersionStore;
    private readonly IUnitOfWork unitOfWork;

    public ResolveConflictCommandHandler(
        ISyncBatchRepository syncBatchRepository,
        IEntityVersionStore entityVersionStore,
        IUnitOfWork unitOfWork)
    {
        this.syncBatchRepository = syncBatchRepository;
        this.entityVersionStore = entityVersionStore;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(ResolveConflictCommand command, CancellationToken cancellationToken = default)
    {
        var conflictId = ConflictRecordIdentifier.From(command.ConflictId);
        var batch = await syncBatchRepository.GetByConflictIdAsync(conflictId, cancellationToken)
            ?? throw new InvalidOperationException($"Batch fuer Konflikt {command.ConflictId} nicht gefunden.");

        var strategy = ConflictResolutionStrategy.From(command.Strategy);
        var mergedPayload = command.MergedPayload is not null
            ? DeltaPayload.From(command.MergedPayload) : null;

        batch.ResolveConflict(conflictId, strategy, mergedPayload);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);

        if (strategy == ConflictResolutionStrategy.ClientWins ||
            strategy == ConflictResolutionStrategy.ManualMerge)
        {
            var resolvedPayload = conflict.ResolvedPayload!;
            var currentVersion = await entityVersionStore.GetCurrentVersionAsync(
                conflict.EntityRef.EntityType, conflict.EntityRef.EntityId, cancellationToken);
            var newVersion = currentVersion.Increment();

            await entityVersionStore.SetVersionAsync(
                conflict.EntityRef.EntityType,
                conflict.EntityRef.EntityId,
                newVersion,
                resolvedPayload.Value,
                batch.DeviceId,
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        SyncMetrics.ConflictsResolved.Add(1);
    }
}
