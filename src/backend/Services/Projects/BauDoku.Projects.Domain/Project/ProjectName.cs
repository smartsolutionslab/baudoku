using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Projects.Domain;

public sealed record ProjectName : IValueObject
{
    public const int MaxLength = 200;
    public string Value { get; }

    private ProjectName(string value) => Value = value;

    public static ProjectName From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Projektname darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Projektname darf max. {MaxLength} Zeichen lang sein.");
        return new ProjectName(value);
    }
}
