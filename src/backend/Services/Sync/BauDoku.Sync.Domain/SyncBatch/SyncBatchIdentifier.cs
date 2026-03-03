using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record SyncBatchIdentifier : IValueObject
{
    public Guid Value { get; }

    private SyncBatchIdentifier(Guid value) => Value = value;

    public static SyncBatchIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("SyncBatch-ID darf nicht leer sein.");
        return new SyncBatchIdentifier(value);
    }

    public static SyncBatchIdentifier New() => new(Guid.NewGuid());
}
