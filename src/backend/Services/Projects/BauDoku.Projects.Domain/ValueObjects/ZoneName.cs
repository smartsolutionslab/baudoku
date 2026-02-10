using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ZoneName : ValueObject
{
    public const int MaxLength = 200;
    public string Value { get; }

    public ZoneName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Zonenname darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Zonenname darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
