using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Manufacturer : ValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    public Manufacturer(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Hersteller darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Hersteller darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
