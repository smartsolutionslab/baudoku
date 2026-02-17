using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record CorrectionService : IValueObject
{
    public const int MaxLength = 20;

    public string Value { get; }

    private CorrectionService(string value) => Value = value;

    public static CorrectionService From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Korrekturdienst darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Korrekturdienst darf max. {MaxLength} Zeichen lang sein.");
        return new CorrectionService(value);
    }
}
