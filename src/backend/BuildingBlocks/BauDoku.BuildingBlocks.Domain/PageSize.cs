using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.Domain;

public sealed record PageSize : IValueObject
{
    public const int Max = 100;

    public static PageSize Default => new(20);

    public int Value { get; }

    private PageSize(int value) => Value = value;

    public static PageSize From(int value)
    {
        Ensure.That(value).IsBetween(1, Max, $"Seitengröße muss zwischen 1 und {Max} liegen.");
        return new PageSize(value);
    }
}
