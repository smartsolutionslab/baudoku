using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

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

    public static MeasurementValue Create(double value, MeasurementUnit unit, double? minThreshold = null, double? maxThreshold = null)
    {
        Ensure.That(unit).IsNotNull("Messeinheit darf nicht null sein.");

        if (minThreshold.HasValue && maxThreshold.HasValue && minThreshold.Value > maxThreshold.Value)
            throw new ArgumentException("MinThreshold darf nicht groesser als MaxThreshold sein.");

        return new MeasurementValue(value, unit, minThreshold, maxThreshold);
    }
}
