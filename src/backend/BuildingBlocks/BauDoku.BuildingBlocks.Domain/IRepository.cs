namespace BauDoku.BuildingBlocks.Domain;

public interface IRepository<T, in TId>
    where T : AggregateRoot<TId>
    where TId : IValueObject
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdReadOnlyAsync(TId id, CancellationToken cancellationToken = default) => GetByIdAsync(id, cancellationToken);
    Task AddAsync(T aggregate, CancellationToken cancellationToken = default);
}
