using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record MeasurementResult : ValueObject
{
    private static readonly HashSet<string> ValidValues =
    [
        "passed", "failed", "warning"
    ];

    public static readonly MeasurementResult Passed = new("passed");
    public static readonly MeasurementResult Failed = new("failed");
    public static readonly MeasurementResult Warning = new("warning");

    public string Value { get; }

    public MeasurementResult(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Messergebnis darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltiges Messergebnis: {value}.", nameof(value));
        Value = value;
    }
}
