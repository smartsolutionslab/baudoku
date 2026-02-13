using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class MeasurementValueMustBeNonNegative : IBusinessRule
{
    private readonly MeasurementValue value;

    public MeasurementValueMustBeNonNegative(MeasurementValue value)
    {
        this.value = value;
    }

    public bool IsBroken() => value.Value < 0;

    public string Message => "Der Messwert darf nicht negativ sein.";
}
