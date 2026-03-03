using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record ProjectIdentifier : IValueObject
{
    public Guid Value { get; }

    private ProjectIdentifier(Guid value) => Value = value;

    public static ProjectIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Projekt-ID darf nicht leer sein.");
        return new ProjectIdentifier(value);
    }

    public static ProjectIdentifier? FromNullable(Guid? value) => value.HasValue ? From(value.Value) : null;

    public static ProjectIdentifier New() => new(Guid.NewGuid());
}
