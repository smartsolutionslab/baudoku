using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class CompletedInstallationCannotBeModified(InstallationStatus currentStatus) : IBusinessRule
{
    public bool IsBroken() => currentStatus == InstallationStatus.Completed || currentStatus == InstallationStatus.Inspected;

    public string Message => "Eine abgeschlossene Installation kann nicht mehr geaendert werden.";
}
