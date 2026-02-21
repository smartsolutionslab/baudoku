using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain;

public sealed record EmailAddress : IValueObject
{
    public const int MaxLength = 254;
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("E-Mail-Adresse darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"E-Mail-Adresse darf max. {MaxLength} Zeichen lang sein.");
        return new EmailAddress(value);
    }

    public static EmailAddress? FromNullable(string? value) => value is null ? null : From(value);
}
