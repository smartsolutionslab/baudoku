using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record MeasurementValue : IValueObject
{
    public const int MaxUnitLength = 20;

    public double Value { get; }
    public string Unit { get; }
    public double? MinThreshold { get; }
    public double? MaxThreshold { get; }

    private MeasurementValue(double value, string unit, double? minThreshold, double? maxThreshold)
    {
        Value = value;
        Unit = unit;
        MinThreshold = minThreshold;
        MaxThreshold = maxThreshold;
    }

    public static MeasurementValue Create(double value, string unit, double? minThreshold = null, double? maxThreshold = null)
    {
        Ensure.That(unit).IsNotNullOrWhiteSpace("Einheit darf nicht leer sein.")
            .MaxLengthIs(MaxUnitLength, $"Einheit darf max. {MaxUnitLength} Zeichen lang sein.");
        return new MeasurementValue(value, unit, minThreshold, maxThreshold);
    }
}
