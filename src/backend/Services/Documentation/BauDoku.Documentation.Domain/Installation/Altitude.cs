using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record Altitude : IValueObject
{
    public double Value { get; }

    private Altitude(double value) => Value = value;

    public static Altitude From(double value) => new(value);

    public static Altitude? FromNullable(double? value) => value.HasValue ? From(value.Value) : null;
}
