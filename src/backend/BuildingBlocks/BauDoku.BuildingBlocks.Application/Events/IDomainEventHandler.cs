using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.BuildingBlocks.Application.Events;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken = default);
}
