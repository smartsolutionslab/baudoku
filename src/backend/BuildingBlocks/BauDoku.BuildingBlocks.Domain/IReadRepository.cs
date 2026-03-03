namespace BauDoku.BuildingBlocks.Domain;

public interface IReadRepository<TDto, in TId> where TId : IValueObject
{
    Task<TDto> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}
