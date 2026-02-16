using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Rules;

public sealed class BatchMustNotBeAlreadyProcessed(BatchStatus currentStatus) : IBusinessRule
{
    public bool IsBroken() =>
        currentStatus == BatchStatus.Completed ||
        currentStatus == BatchStatus.Failed ||
        currentStatus == BatchStatus.PartialConflict;

    public string Message => "Batch wurde bereits verarbeitet und kann nicht erneut verarbeitet werden.";
}
