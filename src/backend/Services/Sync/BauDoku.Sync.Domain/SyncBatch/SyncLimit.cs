using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record SyncLimit : IValueObject
{
    public const int Max = 1000;

    public static SyncLimit Default => new(100);

    public int Value { get; }

    private SyncLimit(int value) => Value = value;

    public static SyncLimit From(int value)
    {
        Ensure.That(value).IsBetween(1, Max, $"Limit muss zwischen 1 und {Max} liegen.");
        return new SyncLimit(value);
    }

    public static SyncLimit FromNullable(int? value) => value.HasValue ? From(value.Value) : Default;
}
