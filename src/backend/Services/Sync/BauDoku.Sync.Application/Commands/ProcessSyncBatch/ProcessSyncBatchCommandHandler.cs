using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Diagnostics;
using BauDoku.Sync.Application.Mapping;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Commands.ProcessSyncBatch;

public sealed class ProcessSyncBatchCommandHandler(ISyncBatchRepository syncBatches, IEntityVersionStore entityVersionStore, IUnitOfWork unitOfWork)
    : ICommandHandler<ProcessSyncBatchCommand, ProcessSyncBatchResult>
{
    public async Task<ProcessSyncBatchResult> Handle(ProcessSyncBatchCommand command, CancellationToken cancellationToken = default)
    {
        var batchId = SyncBatchIdentifier.New();
        var deviceId = DeviceIdentifier.From(command.DeviceId);
        var batch = SyncBatch.Create(batchId, deviceId, DateTime.UtcNow);

        var appliedCount = 0;
        var conflicts = new List<ConflictDto>();

        foreach (var (entityTypeName, entityId, operationName, baseVersion, payloadJson, timestamp) in command.Deltas)
        {
            SyncMetrics.DeltaPayloadSize.Record(payloadJson.Length);

            var entityType = EntityType.From(entityTypeName);
            var entityRef = EntityReference.Create(entityType, entityId);
            var operation = DeltaOperation.From(operationName);
            var clientBaseVersion = SyncVersion.From(baseVersion);
            var payload = DeltaPayload.From(payloadJson);

            var currentServerVersion = await entityVersionStore.GetCurrentVersionAsync(
                entityType, entityId, cancellationToken);

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
                    timestamp);

                await entityVersionStore.SetVersionAsync(
                    entityType, entityId, newVersion,
                    payloadJson, deviceId, cancellationToken);

                appliedCount++;
            }
            else
            {
                var serverPayloadJson = await entityVersionStore.GetCurrentPayloadAsync(
                    entityType, entityId, cancellationToken);
                var serverPayload = DeltaPayload.From(serverPayloadJson ?? "{}");

                var conflict = batch.AddConflict(
                    ConflictRecordIdentifier.New(),
                    entityRef,
                    payload,
                    serverPayload,
                    clientBaseVersion,
                    currentServerVersion);

                conflicts.Add(conflict.ToDto());
            }
        }

        if (conflicts.Count == 0)
            batch.MarkCompleted();
        else if (appliedCount > 0)
            batch.MarkPartialConflict();
        else
            batch.MarkFailed();

        await syncBatches.AddAsync(batch, cancellationToken);
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
