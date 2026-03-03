using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed class InstallationMustHaveValidGpsPosition(GpsPosition position) : IBusinessRule
{
    public const double MaxHorizontalAccuracyMeters = 100;

    public bool IsBroken() => position.HorizontalAccuracy.Value > MaxHorizontalAccuracyMeters;

    public string Message => $"Die GPS-Position muss eine ausreichende Genauigkeit haben (< {MaxHorizontalAccuracyMeters}m).";
}
