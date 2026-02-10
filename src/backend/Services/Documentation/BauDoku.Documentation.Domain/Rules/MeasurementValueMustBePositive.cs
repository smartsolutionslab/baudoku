using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class MeasurementValueMustBePositive : IBusinessRule
{
    private readonly MeasurementValue _value;

    public MeasurementValueMustBePositive(MeasurementValue value)
    {
        _value = value;
    }

    public bool IsBroken() => _value.Value <= 0;

    public string Message => "Der Messwert muss groesser als 0 sein.";
}
