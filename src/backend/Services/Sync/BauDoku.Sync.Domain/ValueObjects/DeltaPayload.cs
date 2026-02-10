using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record DeltaPayload : ValueObject
{
    public const int MaxLength = 1_048_576; // 1 MB

    public string Value { get; }

    public DeltaPayload(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Delta-Payload darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Delta-Payload darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
