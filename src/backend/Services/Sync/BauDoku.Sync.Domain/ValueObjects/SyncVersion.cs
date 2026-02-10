using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record SyncVersion : ValueObject
{
    public long Value { get; }

    public SyncVersion(long value)
    {
        if (value < 0)
            throw new ArgumentException("Sync-Version darf nicht negativ sein.", nameof(value));
        Value = value;
    }

    public static SyncVersion Initial => new(0);

    public SyncVersion Increment() => new(Value + 1);
}
