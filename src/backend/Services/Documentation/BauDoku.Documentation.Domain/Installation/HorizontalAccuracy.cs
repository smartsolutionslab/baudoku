using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record HorizontalAccuracy : IValueObject
{
    public double Value { get; }

    private HorizontalAccuracy(double value) => Value = value;

    public static HorizontalAccuracy From(double value)
    {
        Ensure.That(value).IsPositive("Horizontale Genauigkeit muss größer als 0 sein.");
        return new HorizontalAccuracy(value);
    }
}
