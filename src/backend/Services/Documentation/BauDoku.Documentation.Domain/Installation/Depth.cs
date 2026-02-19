using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record Depth : IValueObject
{
    public int ValueInMillimeters { get; }

    private Depth(int valueInMillimeters) => ValueInMillimeters = valueInMillimeters;

    public static Depth From(int valueInMillimeters)
    {
        Ensure.That(valueInMillimeters).IsNotNegative("Tiefe darf nicht negativ sein.");
        return new Depth(valueInMillimeters);
    }

    public static Depth? FromNullable(int? value) => value.HasValue ? From(value.Value) : null;
}
