using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class InstallationMustHaveValidGpsPosition : IBusinessRule
{
    private readonly GpsPosition position;

    public InstallationMustHaveValidGpsPosition(GpsPosition position)
    {
        this.position = position;
    }

    public bool IsBroken() => position.HorizontalAccuracy > 100;

    public string Message => "Die GPS-Position muss eine ausreichende Genauigkeit haben (< 100m).";
}
