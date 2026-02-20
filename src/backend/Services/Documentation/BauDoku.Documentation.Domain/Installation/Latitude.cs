using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record Latitude : IValueObject
{
    public double Value { get; }

    private Latitude(double value) => Value = value;

    public static Latitude From(double value)
    {
        Ensure.That(value).IsBetween(-90.0, 90.0, "Breitengrad muss zwischen -90 und 90 liegen.");
        return new Latitude(value);
    }

    public static Latitude? FromNullable(double? value) => value.HasValue ? From(value.Value) : null;
}
