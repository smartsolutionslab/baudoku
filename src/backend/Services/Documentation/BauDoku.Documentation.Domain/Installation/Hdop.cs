using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record Hdop : IValueObject
{
    public double Value { get; }

    private Hdop(double value) => Value = value;

    public static Hdop From(double value)
    {
        Ensure.That(value).IsPositive("HDOP muss groesser als 0 sein.");
        return new Hdop(value);
    }

    public static Hdop? FromNullable(double? value) => value.HasValue ? From(value.Value) : null;
}
