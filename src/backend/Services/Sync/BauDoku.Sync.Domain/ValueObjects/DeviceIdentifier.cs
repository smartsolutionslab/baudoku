using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record DeviceIdentifier : IValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    private DeviceIdentifier(string value) => Value = value;

    public static DeviceIdentifier From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Device-ID darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Device-ID darf max. {MaxLength} Zeichen lang sein.");
        return new DeviceIdentifier(value);
    }

    public static DeviceIdentifier? FromNullable(string? value) => value is not null ? From(value) : null;
}
