using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Diagnostics;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Commands.ResolveConflict;

public sealed class ResolveConflictCommandHandler : ICommandHandler<ResolveConflictCommand>
{
    private readonly ISyncBatchRepository _syncBatchRepository;
    private readonly IEntityVersionStore _entityVersionStore;
    private readonly IUnitOfWork _unitOfWork;

    public ResolveConflictCommandHandler(
        ISyncBatchRepository syncBatchRepository,
        IEntityVersionStore entityVersionStore,
        IUnitOfWork unitOfWork)
    {
        _syncBatchRepository = syncBatchRepository;
        _entityVersionStore = entityVersionStore;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResolveConflictCommand command, CancellationToken cancellationToken = default)
    {
        var conflictId = new ConflictRecordId(command.ConflictId);
        var batch = await _syncBatchRepository.GetByConflictIdAsync(conflictId, cancellationToken)
            ?? throw new InvalidOperationException($"Batch fuer Konflikt {command.ConflictId} nicht gefunden.");

        var conflict = batch.Conflicts.FirstOrDefault(c => c.Id == conflictId)
            ?? throw new InvalidOperationException($"Konflikt {command.ConflictId} nicht gefunden.");

        var strategy = new ConflictResolutionStrategy(command.Strategy);
        var mergedPayload = command.MergedPayload is not null
            ? new DeltaPayload(command.MergedPayload) : null;

        conflict.Resolve(strategy, mergedPayload);

        if (strategy == ConflictResolutionStrategy.ClientWins ||
            strategy == ConflictResolutionStrategy.ManualMerge)
        {
            var resolvedPayload = conflict.ResolvedPayload!;
            var currentVersion = await _entityVersionStore.GetCurrentVersionAsync(
                conflict.EntityRef.EntityType, conflict.EntityRef.EntityId, cancellationToken);
            var newVersion = currentVersion.Increment();

            await _entityVersionStore.SetVersionAsync(
                conflict.EntityRef.EntityType,
                conflict.EntityRef.EntityId,
                newVersion,
                resolvedPayload.Value,
                batch.DeviceId,
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        SyncMetrics.ConflictsResolved.Add(1);
    }
}
