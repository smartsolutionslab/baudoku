using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ProjectName : ValueObject
{
    public const int MaxLength = 200;
    public string Value { get; }

    public ProjectName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Projektname darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Projektname darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
