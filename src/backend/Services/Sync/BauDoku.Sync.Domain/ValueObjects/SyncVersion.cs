using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record SyncVersion : ValueObject
{
    public long Value { get; }

    private SyncVersion(long value) => Value = value;

    public static SyncVersion From(long value)
    {
        Ensure.That(value).IsNotNegative("Sync-Version darf nicht negativ sein.");
        return new SyncVersion(value);
    }

    public static SyncVersion Initial => new(0);

    public SyncVersion Increment() => new(Value + 1);
}
