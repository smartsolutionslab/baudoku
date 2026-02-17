using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record MeasurementValue : IValueObject
{
    public double Value { get; }
    public MeasurementUnit Unit { get; }
    public double? MinThreshold { get; }
    public double? MaxThreshold { get; }

    private MeasurementValue(double value, MeasurementUnit unit, double? minThreshold, double? maxThreshold)
    {
        Value = value;
        Unit = unit;
        MinThreshold = minThreshold;
        MaxThreshold = maxThreshold;
    }

    public static MeasurementValue Create(double value, string unit, double? minThreshold = null, double? maxThreshold = null)
    {
        return new MeasurementValue(value, MeasurementUnit.From(unit), minThreshold, maxThreshold);
    }
}
