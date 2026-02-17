namespace BauDoku.BuildingBlocks.Domain;

public abstract class Entity<TIdentity> where TIdentity : IValueObject
{
    public TIdentity Id { get; protected set; } = default!;
}
