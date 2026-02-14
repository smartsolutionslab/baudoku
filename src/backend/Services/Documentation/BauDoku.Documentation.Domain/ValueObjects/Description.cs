using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Description : ValueObject
{
    public const int MaxLength = 2000;

    public string Value { get; }

    private Description(string value) => Value = value;

    public static Description From(string value)
    {
        Ensure.That<string>(value).IsNotNull("Beschreibung darf nicht null sein.");
        Ensure.That(value).MaxLengthIs(MaxLength, $"Beschreibung darf max. {MaxLength} Zeichen lang sein.");
        return new Description(value);
    }
}
