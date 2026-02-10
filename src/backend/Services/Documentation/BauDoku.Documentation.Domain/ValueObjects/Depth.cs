using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Depth : ValueObject
{
    public int ValueInMillimeters { get; }

    public Depth(int valueInMillimeters)
    {
        if (valueInMillimeters < 0)
            throw new ArgumentOutOfRangeException(nameof(valueInMillimeters), "Tiefe darf nicht negativ sein.");
        ValueInMillimeters = valueInMillimeters;
    }
}
