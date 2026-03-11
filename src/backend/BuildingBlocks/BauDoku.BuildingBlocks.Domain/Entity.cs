namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

public abstract class Entity<TIdentity> where TIdentity : IValueObject
{
    public TIdentity Id { get; protected set; } = default!;

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken()) throw new BusinessRuleException(rule);
    }
}
