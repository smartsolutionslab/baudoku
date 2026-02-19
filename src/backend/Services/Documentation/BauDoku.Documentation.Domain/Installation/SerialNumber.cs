using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record SerialNumber : IValueObject
{
    public const int MaxLength = 100;

    public string Value { get; }

    private SerialNumber(string value) => Value = value;

    public static SerialNumber From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Seriennummer darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Seriennummer darf max. {MaxLength} Zeichen lang sein.");
        return new SerialNumber(value);
    }

    public static SerialNumber? FromNullable(string? value) => value is not null ? From(value) : null;
}
