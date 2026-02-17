namespace BauDoku.BuildingBlocks.Domain;

public sealed class BusinessRuleException(IBusinessRule brokenRule) : Exception(brokenRule.Message)
{
    public IBusinessRule BrokenRule { get; } = brokenRule;
}
