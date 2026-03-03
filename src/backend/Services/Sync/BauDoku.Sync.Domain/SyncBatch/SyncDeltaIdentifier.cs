using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record SyncDeltaIdentifier : IValueObject
{
    public Guid Value { get; }

    private SyncDeltaIdentifier(Guid value) => Value = value;

    public static SyncDeltaIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("SyncDelta-ID darf nicht leer sein.");
        return new SyncDeltaIdentifier(value);
    }

    public static SyncDeltaIdentifier New() => new(Guid.NewGuid());
}
