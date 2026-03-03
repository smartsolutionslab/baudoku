using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed class MeasurementValueMustBeNonNegative(MeasurementValue value) : IBusinessRule
{
    public bool IsBroken() => value.Value < 0;

    public string Message => "Der Messwert darf nicht negativ sein.";
}
