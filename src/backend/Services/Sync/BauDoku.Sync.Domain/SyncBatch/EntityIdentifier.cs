using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record EntityIdentifier : IValueObject
{
    public Guid Value { get; }

    private EntityIdentifier(Guid value) => Value = value;

    public static EntityIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Entity-ID darf nicht leer sein.");
        return new EntityIdentifier(value);
    }

    public static EntityIdentifier New() => new(Guid.NewGuid());
}
