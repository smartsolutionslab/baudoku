using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class InstallationMustHaveValidGpsPosition(GpsPosition position) : IBusinessRule
{
    public bool IsBroken() => position.HorizontalAccuracy.Value > 100;

    public string Message => "Die GPS-Position muss eine ausreichende Genauigkeit haben (< 100m).";
}
