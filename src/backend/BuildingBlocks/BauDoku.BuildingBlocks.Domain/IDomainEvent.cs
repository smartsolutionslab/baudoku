namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
