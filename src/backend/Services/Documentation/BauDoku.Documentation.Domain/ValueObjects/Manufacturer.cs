using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Manufacturer : IValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    private Manufacturer(string value) => Value = value;

    public static Manufacturer From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Hersteller darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Hersteller darf max. {MaxLength} Zeichen lang sein.");
        return new Manufacturer(value);
    }

    public static Manufacturer? FromNullable(string? value) => value is not null ? From(value) : null;
}
