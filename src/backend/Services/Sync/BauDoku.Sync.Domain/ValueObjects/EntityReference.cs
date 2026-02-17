using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record EntityReference : ValueObject
{
    public EntityType EntityType { get; }
    public Guid EntityId { get; }

    private EntityReference(EntityType entityType, Guid entityId)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public static EntityReference Create(EntityType entityType, Guid entityId)
    {
        Ensure.That(entityType).IsNotNull("Entity-Typ darf nicht null sein.");
        Ensure.That(entityId).IsNotEmpty("Entity-ID darf nicht leer sein.");
        return new EntityReference(entityType, entityId);
    }

    public void Deconstruct(out EntityType entityType, out Guid entityId)
    {
       entityType = EntityType;
       entityId = EntityId;
    }
}
