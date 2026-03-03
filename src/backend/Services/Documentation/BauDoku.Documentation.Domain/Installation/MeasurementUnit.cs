using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record MeasurementUnit : IValueObject
{
    public const int MaxLength = 20;

    public string Value { get; }

    private MeasurementUnit(string value) => Value = value;

    public static MeasurementUnit From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Einheit darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Einheit darf max. {MaxLength} Zeichen lang sein.");
        return new MeasurementUnit(value);
    }
}
