using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Rules;

public sealed class BatchMustNotBeAlreadyProcessed : IBusinessRule
{
    private readonly BatchStatus _currentStatus;

    public BatchMustNotBeAlreadyProcessed(BatchStatus currentStatus)
    {
        _currentStatus = currentStatus;
    }

    public bool IsBroken() =>
        _currentStatus == BatchStatus.Completed ||
        _currentStatus == BatchStatus.Failed ||
        _currentStatus == BatchStatus.PartialConflict;

    public string Message => "Batch wurde bereits verarbeitet und kann nicht erneut verarbeitet werden.";
}
