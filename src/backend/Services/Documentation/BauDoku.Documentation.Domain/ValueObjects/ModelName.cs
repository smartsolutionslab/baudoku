using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record ModelName : ValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    public ModelName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Modellname darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Modellname darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
