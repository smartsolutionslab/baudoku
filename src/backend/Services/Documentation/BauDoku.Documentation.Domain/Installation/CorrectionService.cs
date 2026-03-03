using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

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

    public static CorrectionService? FromNullable(string? value) => value is not null ? From(value) : null;
}
