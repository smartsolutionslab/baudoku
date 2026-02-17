namespace BauDoku.BuildingBlocks.Domain;

public abstract class Entity<TId> where TId : IValueObject
{
    public TId Id { get; protected set; } = default!;
}
