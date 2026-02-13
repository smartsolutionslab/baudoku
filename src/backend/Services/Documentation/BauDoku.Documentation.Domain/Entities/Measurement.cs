using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Entities;

public sealed class Measurement : Entity<MeasurementIdentifier>
{
    public MeasurementType Type { get; private set; } = default!;
    public MeasurementValue Value { get; private set; } = default!;
    public MeasurementResult Result { get; private set; } = default!;
    public DateTime MeasuredAt { get; private set; }
    public string? Notes { get; private set; }

    private Measurement() { }

    internal static Measurement Create(MeasurementIdentifier id, MeasurementType type, MeasurementValue value, string? notes)
    {
        var result = Evaluate(value);

        return new Measurement
        {
            Id = id,
            Type = type,
            Value = value,
            Result = result,
            MeasuredAt = DateTime.UtcNow,
            Notes = notes
        };
    }

    private static MeasurementResult Evaluate(MeasurementValue value)
    {
        if (value.MinThreshold is null && value.MaxThreshold is null)
            return MeasurementResult.Passed;

        if (value.MinThreshold.HasValue && value.Value < value.MinThreshold.Value)
            return MeasurementResult.Failed;

        if (value.MaxThreshold.HasValue && value.Value > value.MaxThreshold.Value)
            return MeasurementResult.Failed;

        return MeasurementResult.Passed;
    }
}
