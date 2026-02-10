using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Description : ValueObject
{
    public const int MaxLength = 2000;

    public string Value { get; }

    public Description(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Length > MaxLength)
            throw new ArgumentException($"Beschreibung darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
