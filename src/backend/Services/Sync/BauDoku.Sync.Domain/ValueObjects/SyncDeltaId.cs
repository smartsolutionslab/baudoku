using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record SyncDeltaId : ValueObject
{
    public Guid Value { get; }

    public SyncDeltaId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("SyncDelta-ID darf nicht leer sein.", nameof(value));
        Value = value;
    }

    public static SyncDeltaId New() => new(Guid.NewGuid());
}
