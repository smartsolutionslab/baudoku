using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Rules;

public sealed class ConflictMustBeUnresolved : IBusinessRule
{
    private readonly ConflictStatus currentStatus;

    public ConflictMustBeUnresolved(ConflictStatus currentStatus)
    {
        this.currentStatus = currentStatus;
    }

    public bool IsBroken() =>
        currentStatus != ConflictStatus.Unresolved;

    public string Message => "Nur ungeloeste Konflikte koennen aufgeloest werden.";
}
