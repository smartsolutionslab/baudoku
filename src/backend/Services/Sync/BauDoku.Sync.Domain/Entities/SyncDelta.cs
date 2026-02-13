using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Entities;

public sealed class SyncDelta : Entity<SyncDeltaIdentifier>
{
    public EntityReference EntityRef { get; private set; } = default!;
    public DeltaOperation Operation { get; private set; } = default!;
    public SyncVersion BaseVersion { get; private set; } = default!;
    public SyncVersion ServerVersion { get; private set; } = default!;
    public DeltaPayload Payload { get; private set; } = default!;
    public DateTime Timestamp { get; private set; }

    private SyncDelta() { }

    internal static SyncDelta Create(
        SyncDeltaIdentifier id,
        EntityReference entityRef,
        DeltaOperation operation,
        SyncVersion baseVersion,
        SyncVersion serverVersion,
        DeltaPayload payload,
        DateTime timestamp)
    {
        return new SyncDelta
        {
            Id = id,
            EntityRef = entityRef,
            Operation = operation,
            BaseVersion = baseVersion,
            ServerVersion = serverVersion,
            Payload = payload,
            Timestamp = timestamp
        };
    }
}
