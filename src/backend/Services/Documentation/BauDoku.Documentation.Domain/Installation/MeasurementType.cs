using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record MeasurementType : IValueObject
{
    private static readonly HashSet<string> ValidValues =
    [
        "insulation_resistance", "continuity", "loop_impedance",
        "rcd_trip_time", "rcd_trip_current", "voltage", "other"
    ];

    public static readonly MeasurementType InsulationResistance = new("insulation_resistance");
    public static readonly MeasurementType Continuity = new("continuity");
    public static readonly MeasurementType LoopImpedance = new("loop_impedance");
    public static readonly MeasurementType RcdTripTime = new("rcd_trip_time");
    public static readonly MeasurementType RcdTripCurrent = new("rcd_trip_current");
    public static readonly MeasurementType Voltage = new("voltage");
    public static readonly MeasurementType Other = new("other");

    public string Value { get; }

    private MeasurementType(string value) => Value = value;

    public static MeasurementType From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Messungstyp darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Messungstyp: {value}.");
        return new MeasurementType(value);
    }
}
