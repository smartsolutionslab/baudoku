using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record DeviceId : ValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    public DeviceId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Device-ID darf nicht leer sein.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Device-ID darf max. {MaxLength} Zeichen lang sein.", nameof(value));
        Value = value;
    }
}
