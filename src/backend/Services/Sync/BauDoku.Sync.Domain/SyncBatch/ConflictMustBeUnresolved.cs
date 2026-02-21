using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain;

public sealed class ConflictMustBeUnresolved(ConflictStatus currentStatus) : IBusinessRule
{
    public bool IsBroken() => currentStatus != ConflictStatus.Unresolved;

    public string Message => "Nur ungeloeste Konflikte koennen aufgeloest werden.";
}
