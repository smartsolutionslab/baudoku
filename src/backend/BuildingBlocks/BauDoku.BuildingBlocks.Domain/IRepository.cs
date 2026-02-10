namespace BauDoku.BuildingBlocks.Domain;

public interface IRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : ValueObject
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task AddAsync(T aggregate, CancellationToken cancellationToken = default);
}
