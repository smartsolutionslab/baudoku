using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record ModelName : ValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    private ModelName(string value) => Value = value;

    public static ModelName From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Modellname darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Modellname darf max. {MaxLength} Zeichen lang sein.");
        return new ModelName(value);
    }
}
