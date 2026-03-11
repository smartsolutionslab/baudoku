using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record Altitude : IValueObject
{
    public double Value { get; }

    private Altitude(double value) => Value = value;

    public static Altitude From(double value)
    {
        Ensure.That(value).IsBetween(-11_000.0, 100_000.0, "Hoehe muss zwischen -11.000 und 100.000 Metern liegen.");
        return new(value);
    }

    public static Altitude? FromNullable(double? value) => value.HasValue ? From(value.Value) : null;
}
