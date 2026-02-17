namespace BauDoku.BuildingBlocks.Domain;

public abstract class AggregateRoot<TIdentity> : Entity<TIdentity>, IAggregateRoot where TIdentity : IValueObject
{
    private readonly List<IDomainEvent> domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => domainEvents.Clear();

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken()) throw new BusinessRuleException(rule);
    }
}
