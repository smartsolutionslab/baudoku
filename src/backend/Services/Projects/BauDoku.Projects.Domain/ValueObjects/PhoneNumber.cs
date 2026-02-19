using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record PhoneNumber : IValueObject
{
    public const int MaxLength = 30;
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Telefonnummer darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Telefonnummer darf max. {MaxLength} Zeichen lang sein.");
        return new PhoneNumber(value);
    }

    public static PhoneNumber? FromNullable(string? value) => value is null ? null : From(value);
}
