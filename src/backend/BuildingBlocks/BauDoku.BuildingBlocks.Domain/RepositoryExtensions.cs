namespace BauDoku.BuildingBlocks.Domain;

public static class RepositoryExtensions
{
    public static Task<TAggregateRoot> With<TAggregateRoot, TIdentifier>(
        this IRepository<TAggregateRoot, TIdentifier> repository,
        TIdentifier id,
        CancellationToken cancellationToken = default)
        where TAggregateRoot : AggregateRoot<TIdentifier>
        where TIdentifier : IValueObject
        => repository.GetByIdAsync(id, cancellationToken);

    public static Task<TAggregateRoot> With<TAggregateRoot, TIdentifier>(
        this IEventSourcedRepository<TAggregateRoot, TIdentifier> repository,
        TIdentifier id,
        CancellationToken cancellationToken = default)
        where TAggregateRoot : EventSourcedAggregateRoot<TIdentifier>
        where TIdentifier : IValueObject
        => repository.GetByIdAsync(id, cancellationToken);

    public static Task<TDto> With<TDto, TId>(this IReadRepository<TDto, TId> repository, TId id, CancellationToken cancellationToken = default)
        where TId : IValueObject
        => repository.GetByIdAsync(id, cancellationToken);
}
