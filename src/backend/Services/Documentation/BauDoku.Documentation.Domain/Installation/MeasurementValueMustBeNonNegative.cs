using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class MeasurementValueMustBeNonNegative(MeasurementValue value) : IBusinessRule
{
    public bool IsBroken() => value.Value < 0;

    public string Message => "Der Messwert darf nicht negativ sein.";
}
