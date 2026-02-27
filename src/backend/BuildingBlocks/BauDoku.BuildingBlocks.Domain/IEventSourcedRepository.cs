namespace BauDoku.BuildingBlocks.Domain;

public interface IEventSourcedRepository<TAggregateRoot, in TIdentifier>
    where TAggregateRoot : EventSourcedAggregateRoot<TIdentifier>
    where TIdentifier : IValueObject
{
    Task<TAggregateRoot> GetByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);
    Task SaveAsync(TAggregateRoot aggregate, CancellationToken cancellationToken = default);
}
