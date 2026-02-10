using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class CompletedInstallationCannotBeModified : IBusinessRule
{
    private readonly InstallationStatus _currentStatus;

    public CompletedInstallationCannotBeModified(InstallationStatus currentStatus)
    {
        _currentStatus = currentStatus;
    }

    public bool IsBroken() =>
        _currentStatus == InstallationStatus.Completed ||
        _currentStatus == InstallationStatus.Inspected;

    public string Message => "Eine abgeschlossene Installation kann nicht mehr geaendert werden.";
}
