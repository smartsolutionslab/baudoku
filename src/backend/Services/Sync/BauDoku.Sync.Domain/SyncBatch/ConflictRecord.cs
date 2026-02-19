using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.Rules;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Entities;

public sealed class ConflictRecord : Entity<ConflictRecordIdentifier>
{
    public EntityReference EntityRef { get; private set; } = default!;
    public DeltaPayload ClientPayload { get; private set; } = default!;
    public DeltaPayload ServerPayload { get; private set; } = default!;
    public SyncVersion ClientVersion { get; private set; } = default!;
    public SyncVersion ServerVersion { get; private set; } = default!;
    public ConflictStatus Status { get; private set; } = default!;
    public DeltaPayload? ResolvedPayload { get; private set; }
    public DateTime DetectedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    private ConflictRecord() { }

    internal static ConflictRecord Create(
        ConflictRecordIdentifier id,
        EntityReference entityRef,
        DeltaPayload clientPayload,
        DeltaPayload serverPayload,
        SyncVersion clientVersion,
        SyncVersion serverVersion)
    {
        return new ConflictRecord
        {
            Id = id,
            EntityRef = entityRef,
            ClientPayload = clientPayload,
            ServerPayload = serverPayload,
            ClientVersion = clientVersion,
            ServerVersion = serverVersion,
            Status = ConflictStatus.Unresolved,
            DetectedAt = DateTime.UtcNow
        };
    }

    public void Resolve(ConflictResolutionStrategy strategy, DeltaPayload? mergedPayload = null)
    {
        var rule = new ConflictMustBeUnresolved(Status);
        if (rule.IsBroken()) throw new BusinessRuleException(rule);

        Status = strategy == ConflictResolutionStrategy.ClientWins
            ? ConflictStatus.ClientWins
            : strategy == ConflictResolutionStrategy.ServerWins
                ? ConflictStatus.ServerWins
                : ConflictStatus.Merged;

        ResolvedPayload = strategy == ConflictResolutionStrategy.ManualMerge
            ? mergedPayload ?? throw new ArgumentException("Merged-Payload wird bei ManualMerge benoetigt.", nameof(mergedPayload))
            : strategy == ConflictResolutionStrategy.ClientWins
                ? ClientPayload
                : ServerPayload;

        ResolvedAt = DateTime.UtcNow;
    }
}
