using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Entities;

public sealed class Measurement : Entity<MeasurementId>
{
    public MeasurementType Type { get; private set; } = default!;
    public MeasurementValue Value { get; private set; } = default!;
    public DateTime MeasuredAt { get; private set; }
    public string? Notes { get; private set; }

    private Measurement() { }

    internal static Measurement Create(MeasurementId id, MeasurementType type, MeasurementValue value, string? notes)
    {
        return new Measurement
        {
            Id = id,
            Type = type,
            Value = value,
            MeasuredAt = DateTime.UtcNow,
            Notes = notes
        };
    }
}
