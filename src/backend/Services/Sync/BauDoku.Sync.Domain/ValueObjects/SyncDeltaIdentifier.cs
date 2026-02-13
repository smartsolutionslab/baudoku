using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record SyncDeltaIdentifier : ValueObject
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
