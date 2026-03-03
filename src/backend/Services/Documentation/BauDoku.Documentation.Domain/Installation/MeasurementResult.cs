using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record MeasurementResult : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["passed", "failed", "warning"];

    public static readonly MeasurementResult Passed = new("passed");
    public static readonly MeasurementResult Failed = new("failed");
    public static readonly MeasurementResult Warning = new("warning");

    public string Value { get; }

    private MeasurementResult(string value) => Value = value;

    public static MeasurementResult From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Messergebnis darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungueltiges Messergebnis: {value}.");
        return new MeasurementResult(value);
    }
}
