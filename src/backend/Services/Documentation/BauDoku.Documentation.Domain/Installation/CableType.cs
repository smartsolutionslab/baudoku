using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record CableType : IValueObject
{
    public const int MaxLength = 100;

    public string Value { get; }

    private CableType(string value) => Value = value;

    public static CableType From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Kabeltyp darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Kabeltyp darf max. {MaxLength} Zeichen lang sein.");
        return new CableType(value);
    }
}
