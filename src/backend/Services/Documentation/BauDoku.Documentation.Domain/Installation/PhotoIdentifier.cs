using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record PhotoIdentifier : IValueObject
{
    public Guid Value { get; }

    private PhotoIdentifier(Guid value) => Value = value;

    public static PhotoIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Foto-ID darf nicht leer sein.");
        return new PhotoIdentifier(value);
    }

    public static PhotoIdentifier New() => new(Guid.NewGuid());
}
