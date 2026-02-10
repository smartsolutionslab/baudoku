using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Rules;

public sealed class ConflictMustBeUnresolved : IBusinessRule
{
    private readonly ConflictStatus _currentStatus;

    public ConflictMustBeUnresolved(ConflictStatus currentStatus)
    {
        _currentStatus = currentStatus;
    }

    public bool IsBroken() =>
        _currentStatus != ConflictStatus.Unresolved;

    public string Message => "Nur ungeloeste Konflikte koennen aufgeloest werden.";
}
