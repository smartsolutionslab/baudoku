using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class CompletedInstallationCannotBeModified : IBusinessRule
{
    private readonly InstallationStatus currentStatus;

    public CompletedInstallationCannotBeModified(InstallationStatus currentStatus)
    {
        this.currentStatus = currentStatus;
    }

    public bool IsBroken() =>
        currentStatus == InstallationStatus.Completed ||
        currentStatus == InstallationStatus.Inspected;

    public string Message => "Eine abgeschlossene Installation kann nicht mehr geaendert werden.";
}
