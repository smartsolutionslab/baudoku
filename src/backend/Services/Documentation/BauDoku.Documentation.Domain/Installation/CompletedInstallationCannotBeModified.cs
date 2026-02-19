using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed class CompletedInstallationCannotBeModified(InstallationStatus currentStatus) : IBusinessRule
{
    public bool IsBroken() => currentStatus == InstallationStatus.Completed || currentStatus == InstallationStatus.Inspected;

    public string Message => "Eine abgeschlossene Installation kann nicht mehr geaendert werden.";
}
