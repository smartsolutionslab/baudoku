namespace BauDoku.BuildingBlocks.Domain;

public abstract class Entity<TId> where TId : ValueObject
{
    public TId Id { get; protected set; } = default!;
}
