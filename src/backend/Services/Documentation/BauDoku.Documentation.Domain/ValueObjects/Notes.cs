using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record Notes : IValueObject
{
    public const int MaxLength = 500;

    public string Value { get; }

    private Notes(string value) => Value = value;

    public static Notes From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Notizen duerfen nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Notizen duerfen max. {MaxLength} Zeichen lang sein.");
        return new Notes(value);
    }
}
