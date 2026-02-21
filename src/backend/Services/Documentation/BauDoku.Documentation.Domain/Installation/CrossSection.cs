using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record CrossSection : IValueObject
{
    public decimal Value { get; }

    private CrossSection(decimal value) => Value = value;

    public static CrossSection From(decimal value)
    {
        Ensure.That(value).IsPositive("Querschnitt muss größer als 0 sein.");
        return new CrossSection(value);
    }

    public static CrossSection? FromNullable(decimal? value) => value.HasValue ? From(value.Value) : null;
}
