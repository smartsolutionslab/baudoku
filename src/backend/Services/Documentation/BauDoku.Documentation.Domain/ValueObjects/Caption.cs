using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Caption : IValueObject
{
    public const int MaxLength = 500;

    public string Value { get; }

    private Caption(string value) => Value = value;

    public static Caption From(string value)
    {
        Ensure.That<string>(value).IsNotNull("Bildunterschrift darf nicht null sein.");
        Ensure.That(value).IsNotNullOrWhiteSpace("Bildunterschrift darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Bildunterschrift darf max. {MaxLength} Zeichen lang sein.");
        return new Caption(value);
    }
}
