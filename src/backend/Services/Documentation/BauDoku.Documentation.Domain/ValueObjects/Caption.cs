using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Caption : ValueObject
{
    public const int MaxLength = 500;

    public string Value { get; }

    public Caption(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Bildunterschrift darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Bildunterschrift darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
