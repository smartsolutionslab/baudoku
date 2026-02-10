namespace BauDoku.BuildingBlocks.Domain;

public interface IRepository<T> where T : AggregateRoot<ValueObject>
{
}
