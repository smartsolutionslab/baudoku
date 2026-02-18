using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record CableColor : IValueObject
{
    public const int MaxLength = 50;

    public string Value { get; }

    private CableColor(string value) => Value = value;

    public static CableColor From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Kabelfarbe darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Kabelfarbe darf max. {MaxLength} Zeichen lang sein.");
        return new CableColor(value);
    }

    public static CableColor? FromNullable(string? value) => value is not null ? From(value) : null;
}
