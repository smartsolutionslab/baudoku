namespace BauDoku.BuildingBlocks.Domain;

public interface IRepository<TAggregateRoot, in TIdentifier>
    where TAggregateRoot : AggregateRoot<TIdentifier>
    where TIdentifier : IValueObject
{
    Task<TAggregateRoot> GetByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);
    Task AddAsync(TAggregateRoot aggregate, CancellationToken cancellationToken = default);
}
