using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain;

public sealed record DeltaPayload : IValueObject
{
    public const int MaxLength = 1_048_576; // 1 MB

    public string Value { get; }

    private DeltaPayload(string value) => Value = value;

    public static DeltaPayload From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Delta-Payload darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Delta-Payload darf max. {MaxLength} Zeichen lang sein.");
        return new DeltaPayload(value);
    }

    public static DeltaPayload? FromNullable(string? value) => value is not null ? From(value) : null;
}
