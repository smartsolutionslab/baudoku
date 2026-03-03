using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record FileName : IValueObject
{
    public const int MaxLength = 255;

    public string Value { get; }

    private FileName(string value) => Value = value;

    public static FileName From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Dateiname darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Dateiname darf max. {MaxLength} Zeichen lang sein.");
        return new FileName(value);
    }
}
