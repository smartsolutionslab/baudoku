using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record MeasurementValue : ValueObject
{
    public const int MaxUnitLength = 20;

    public double Value { get; }
    public string Unit { get; }

    public MeasurementValue(double value, string unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Einheit darf nicht leer sein.", nameof(unit));
        if (unit.Length > MaxUnitLength)
            throw new ArgumentException($"Einheit darf max. {MaxUnitLength} Zeichen lang sein.", nameof(unit));
        Value = value;
        Unit = unit;
    }
}
