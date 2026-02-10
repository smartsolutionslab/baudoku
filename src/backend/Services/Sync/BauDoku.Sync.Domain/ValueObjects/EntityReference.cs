using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record EntityReference : ValueObject
{
    public EntityType EntityType { get; }
    public Guid EntityId { get; }

    public EntityReference(EntityType entityType, Guid entityId)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        if (entityId == Guid.Empty)
            throw new ArgumentException("Entity-ID darf nicht leer sein.", nameof(entityId));
        EntityId = entityId;
    }
}
