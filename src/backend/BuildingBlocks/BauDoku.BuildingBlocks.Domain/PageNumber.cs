using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.Domain;

public sealed record PageNumber : IValueObject
{
    public static PageNumber Default => new(1);

    public int Value { get; }

    private PageNumber(int value) => Value = value;

    public static PageNumber From(int value)
    {
        Ensure.That(value).IsPositive("Seitennummer muss mindestens 1 sein.");
        return new PageNumber(value);
    }

    public static PageNumber? FromNullable(int? value) => value.HasValue ? From(value.Value) : null;
}
