using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Rules;

public sealed class ConflictMustBeUnresolved(ConflictStatus currentStatus) : IBusinessRule
{
    public bool IsBroken() =>
        currentStatus != ConflictStatus.Unresolved;

    public string Message => "Nur ungeloeste Konflikte koennen aufgeloest werden.";
}
