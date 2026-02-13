using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Diagnostics;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Commands.ProcessSyncBatch;

public sealed class ProcessSyncBatchCommandHandler
    : ICommandHandler<ProcessSyncBatchCommand, ProcessSyncBatchResult>
{
    private readonly ISyncBatchRepository syncBatchRepository;
    private readonly IEntityVersionStore entityVersionStore;
    private readonly IUnitOfWork unitOfWork;

    public ProcessSyncBatchCommandHandler(
        ISyncBatchRepository syncBatchRepository,
        IEntityVersionStore entityVersionStore,
        IUnitOfWork unitOfWork)
    {
        this.syncBatchRepository = syncBatchRepository;
        this.entityVersionStore = entityVersionStore;
        this.unitOfWork = unitOfWork;
    }

    public async Task<ProcessSyncBatchResult> Handle(
        ProcessSyncBatchCommand command,
        CancellationToken cancellationToken = default)
    {
        var batchId = SyncBatchIdentifier.New();
        var deviceId = DeviceIdentifier.From(command.DeviceId);
        var batch = SyncBatch.Create(batchId, deviceId, DateTime.UtcNow);

        var appliedCount = 0;
        var conflicts = new List<ConflictDto>();

        foreach (var deltaDto in command.Deltas)
        {
            SyncMetrics.DeltaPayloadSize.Record(deltaDto.Payload.Length);

            var entityType = EntityType.From(deltaDto.EntityType);
            var entityRef = EntityReference.Create(entityType, deltaDto.EntityId);
            var operation = DeltaOperation.From(deltaDto.Operation);
            var clientBaseVersion = SyncVersion.From(deltaDto.BaseVersion);
            var payload = DeltaPayload.From(deltaDto.Payload);

            var currentServerVersion = await entityVersionStore.GetCurrentVersionAsync(
                entityType, deltaDto.EntityId, cancellationToken);

            if (clientBaseVersion.Value == currentServerVersion.Value)
            {
                var newVersion = currentServerVersion.Increment();

                batch.AddDelta(
                    SyncDeltaIdentifier.New(),
                    entityRef,
                    operation,
                    clientBaseVersion,
                    newVersion,
                    payload,
                    deltaDto.Timestamp);

                await entityVersionStore.SetVersionAsync(
                    entityType, deltaDto.EntityId, newVersion,
                    deltaDto.Payload, deviceId, cancellationToken);

                appliedCount++;
            }
            else
            {
                var serverPayloadJson = await entityVersionStore.GetCurrentPayloadAsync(
                    entityType, deltaDto.EntityId, cancellationToken);
                var serverPayload = DeltaPayload.From(serverPayloadJson ?? "{}");

                var conflict = batch.AddConflict(
                    ConflictRecordIdentifier.New(),
                    entityRef,
                    payload,
                    serverPayload,
                    clientBaseVersion,
                    currentServerVersion);

                conflicts.Add(new ConflictDto(
                    conflict.Id.Value,
                    entityType.Value,
                    deltaDto.EntityId,
                    deltaDto.Payload,
                    serverPayload.Value,
                    clientBaseVersion.Value,
                    currentServerVersion.Value,
                    conflict.Status.Value,
                    conflict.DetectedAt));
            }
        }

        if (conflicts.Count == 0)
            batch.MarkCompleted();
        else if (appliedCount > 0)
            batch.MarkPartialConflict();
        else
            batch.MarkFailed();

        await syncBatchRepository.AddAsync(batch, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        SyncMetrics.BatchesProcessed.Add(1);
        SyncMetrics.DeltasApplied.Add(appliedCount);
        SyncMetrics.ConflictsDetected.Add(conflicts.Count);
        SyncMetrics.DeltasPerBatch.Record(command.Deltas.Count);

        return new ProcessSyncBatchResult(
            batchId.Value,
            appliedCount,
            conflicts.Count,
            conflicts);
    }
}
