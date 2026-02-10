using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record SerialNumber : ValueObject
{
    public const int MaxLength = 100;

    public string Value { get; }

    public SerialNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Seriennummer darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Seriennummer darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
