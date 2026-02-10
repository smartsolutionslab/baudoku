using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record SyncBatchId : ValueObject
{
    public Guid Value { get; }

    public SyncBatchId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("SyncBatch-ID darf nicht leer sein.", nameof(value));
        Value = value;
    }

    public static SyncBatchId New() => new(Guid.NewGuid());
}
