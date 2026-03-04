using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record MeasurementType : IValueObject
{
    public static readonly MeasurementType InsulationResistance = new("insulation_resistance");
    public static readonly MeasurementType Continuity = new("continuity");
    public static readonly MeasurementType LoopImpedance = new("loop_impedance");
    public static readonly MeasurementType RcdTripTime = new("rcd_trip_time");
    public static readonly MeasurementType RcdTripCurrent = new("rcd_trip_current");
    public static readonly MeasurementType Voltage = new("voltage");
    public static readonly MeasurementType Other = new("other");

    public static IEnumerable<MeasurementType> All { get; } =
    [
        InsulationResistance,
        Continuity,
        LoopImpedance,
        RcdTripTime,
        RcdTripCurrent,
        Voltage,
        Other
    ];

    private static HashSet<string> ValidValues => All.Select(item => item.Value).ToHashSet();

    public string Value { get; }

    private MeasurementType(string value) => Value = value;

    public static MeasurementType From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Messungstyp darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Messungstyp: {value}.");
        return new MeasurementType(value);
    }
}
