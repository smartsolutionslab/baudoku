using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Guards;

namespace BauDoku.Sync.Domain;

public sealed record EntityReference : IValueObject
{
    public EntityType EntityType { get; }
    public EntityIdentifier EntityId { get; }

    private EntityReference(EntityType entityType, EntityIdentifier entityId)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public static EntityReference Create(EntityType entityType, EntityIdentifier entityId)
    {
        Ensure.That(entityType).IsNotNull("Entity-Typ darf nicht null sein.");
        Ensure.That(entityId).IsNotNull("Entity-ID darf nicht null sein.");
        return new EntityReference(entityType, entityId);
    }

    public void Deconstruct(out EntityType entityType, out EntityIdentifier entityId)
    {
       entityType = EntityType;
       entityId = EntityId;
    }
}
