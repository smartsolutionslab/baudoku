using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.Domain;

public abstract class EventSourcedAggregateRoot<TIdentity> : Entity<TIdentity>, IAggregateRoot
    where TIdentity : IValueObject
{
    private readonly List<IDomainEvent> domainEvents = [];

    public long Version { get; internal set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    protected void RaiseEvent(IDomainEvent domainEvent)
    {
        Ensure.That(domainEvent).IsNotNull("Domain-Event darf nicht null sein.");
        Apply(domainEvent);
        domainEvents.Add(domainEvent);
    }

    public abstract void Apply(IDomainEvent @event);

    public void LoadFromHistory(IReadOnlyList<IDomainEvent> history, long version)
    {
        foreach (var @event in history)
            Apply(@event);
        Version = version;
    }

    public void ClearDomainEvents() => domainEvents.Clear();

    protected static void CheckRule(IBusinessRule rule)
    {
        Ensure.That(rule).IsNotNull("Business-Rule darf nicht null sein.");
        if (rule.IsBroken()) throw new BusinessRuleException(rule);
    }
}
