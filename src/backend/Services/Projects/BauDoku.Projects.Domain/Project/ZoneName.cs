using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Projects.Domain;

public sealed record ZoneName : IValueObject
{
    public const int MaxLength = 200;
    public string Value { get; }

    private ZoneName(string value) => Value = value;

    public static ZoneName From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Zonenname darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Zonenname darf max. {MaxLength} Zeichen lang sein.");
        return new ZoneName(value);
    }
}
