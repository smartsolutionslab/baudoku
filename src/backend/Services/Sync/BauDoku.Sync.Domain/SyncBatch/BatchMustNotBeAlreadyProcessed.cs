using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain;

public sealed class BatchMustNotBeAlreadyProcessed(BatchStatus currentStatus) : IBusinessRule
{
    public bool IsBroken() => currentStatus == BatchStatus.Completed || currentStatus == BatchStatus.Failed || currentStatus == BatchStatus.PartialConflict;

    public string Message => "Batch wurde bereits verarbeitet und kann nicht erneut verarbeitet werden.";
}
