using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record SyncVersion : IValueObject
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
