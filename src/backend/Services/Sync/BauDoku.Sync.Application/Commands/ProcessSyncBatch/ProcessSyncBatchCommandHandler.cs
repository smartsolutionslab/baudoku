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
    private readonly ISyncBatchRepository _syncBatchRepository;
    private readonly IEntityVersionStore _entityVersionStore;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessSyncBatchCommandHandler(
        ISyncBatchRepository syncBatchRepository,
        IEntityVersionStore entityVersionStore,
        IUnitOfWork unitOfWork)
    {
        _syncBatchRepository = syncBatchRepository;
        _entityVersionStore = entityVersionStore;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcessSyncBatchResult> Handle(
        ProcessSyncBatchCommand command,
        CancellationToken cancellationToken = default)
    {
        var batchId = SyncBatchId.New();
        var deviceId = new DeviceId(command.DeviceId);
        var batch = SyncBatch.Create(batchId, deviceId, DateTime.UtcNow);

        var appliedCount = 0;
        var conflicts = new List<ConflictDto>();

        foreach (var deltaDto in command.Deltas)
        {
            SyncMetrics.DeltaPayloadSize.Record(deltaDto.Payload.Length);

            var entityType = new EntityType(deltaDto.EntityType);
            var entityRef = new EntityReference(entityType, deltaDto.EntityId);
            var operation = new DeltaOperation(deltaDto.Operation);
            var clientBaseVersion = new SyncVersion(deltaDto.BaseVersion);
            var payload = new DeltaPayload(deltaDto.Payload);

            var currentServerVersion = await _entityVersionStore.GetCurrentVersionAsync(
                entityType, deltaDto.EntityId, cancellationToken);

            if (clientBaseVersion.Value == currentServerVersion.Value)
            {
                var newVersion = currentServerVersion.Increment();

                batch.AddDelta(
                    SyncDeltaId.New(),
                    entityRef,
                    operation,
                    clientBaseVersion,
                    newVersion,
                    payload,
                    deltaDto.Timestamp);

                await _entityVersionStore.SetVersionAsync(
                    entityType, deltaDto.EntityId, newVersion,
                    deltaDto.Payload, deviceId, cancellationToken);

                appliedCount++;
            }
            else
            {
                var serverPayloadJson = await _entityVersionStore.GetCurrentPayloadAsync(
                    entityType, deltaDto.EntityId, cancellationToken);
                var serverPayload = new DeltaPayload(serverPayloadJson ?? "{}");

                var conflict = batch.AddConflict(
                    ConflictRecordId.New(),
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

        await _syncBatchRepository.AddAsync(batch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
