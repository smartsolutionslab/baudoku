using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record RadiusMeters : IValueObject
{
    public double Value { get; }

    private RadiusMeters(double value) => Value = value;

    public static RadiusMeters From(double value)
    {
        Ensure.That(value).IsPositive("Radius muss groesser als 0 sein.");
        return new RadiusMeters(value);
    }
}
