using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record GpsQualityGrade : ValueObject
{
    private static readonly HashSet<string> ValidValues =
        ["a", "b", "c", "d"];

    public static readonly GpsQualityGrade A = new("a");
    public static readonly GpsQualityGrade B = new("b");
    public static readonly GpsQualityGrade C = new("c");
    public static readonly GpsQualityGrade D = new("d");

    public string Value { get; }

    public GpsQualityGrade(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("GPS-Qualitaetsstufe darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltige GPS-Qualitaetsstufe: {value}.", nameof(value));
        Value = value;
    }
}
