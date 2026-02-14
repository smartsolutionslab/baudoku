using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Depth : ValueObject
{
    public int ValueInMillimeters { get; }

    private Depth(int valueInMillimeters) => ValueInMillimeters = valueInMillimeters;

    public static Depth From(int valueInMillimeters)
    {
        Ensure.That(valueInMillimeters).IsNotNegative("Tiefe darf nicht negativ sein.");
        return new Depth(valueInMillimeters);
    }
}
