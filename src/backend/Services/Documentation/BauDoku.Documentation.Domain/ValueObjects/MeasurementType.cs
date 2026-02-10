using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record MeasurementType : ValueObject
{
    private static readonly HashSet<string> ValidValues =
    [
        "insulation_resistance", "continuity", "loop_impedance",
        "rcd_trip_time", "voltage", "other"
    ];

    public static readonly MeasurementType InsulationResistance = new("insulation_resistance");
    public static readonly MeasurementType Continuity = new("continuity");
    public static readonly MeasurementType LoopImpedance = new("loop_impedance");
    public static readonly MeasurementType RcdTripTime = new("rcd_trip_time");
    public static readonly MeasurementType Voltage = new("voltage");
    public static readonly MeasurementType Other = new("other");

    public string Value { get; }

    public MeasurementType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Messungstyp darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltiger Messungstyp: {value}.", nameof(value));
        Value = value;
    }
}
