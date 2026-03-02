using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record Longitude : IValueObject
{
    public double Value { get; }

    private Longitude(double value) => Value = value;

    public static Longitude From(double value)
    {
        Ensure.That(value).IsBetween(-180.0, 180.0, "Längengrad muss zwischen -180 und 180 liegen.");
        return new Longitude(value);
    }

    public static Longitude? FromNullable(double? value) => value.HasValue ? From(value.Value) : null;
}
