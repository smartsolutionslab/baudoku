namespace BauDoku.BuildingBlocks.Domain;

public sealed class BusinessRuleException : Exception
{
    public IBusinessRule BrokenRule { get; }

    public BusinessRuleException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
    }
}
