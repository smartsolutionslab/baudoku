using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Rules;

public sealed class BatchMustNotBeAlreadyProcessed : IBusinessRule
{
    private readonly BatchStatus currentStatus;

    public BatchMustNotBeAlreadyProcessed(BatchStatus currentStatus)
    {
        this.currentStatus = currentStatus;
    }

    public bool IsBroken() =>
        currentStatus == BatchStatus.Completed ||
        currentStatus == BatchStatus.Failed ||
        currentStatus == BatchStatus.PartialConflict;

    public string Message => "Batch wurde bereits verarbeitet und kann nicht erneut verarbeitet werden.";
}
