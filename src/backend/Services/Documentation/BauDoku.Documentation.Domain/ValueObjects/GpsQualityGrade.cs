using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record GpsQualityGrade : ValueObject
{
    private static readonly HashSet<string> ValidValues = ["a", "b", "c", "d"];

    public static readonly GpsQualityGrade A = new("a");
    public static readonly GpsQualityGrade B = new("b");
    public static readonly GpsQualityGrade C = new("c");
    public static readonly GpsQualityGrade D = new("d");

    public string Value { get; }

    private GpsQualityGrade(string value) => Value = value;

    public static GpsQualityGrade From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("GPS-Qualitaetsstufe darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungueltige GPS-Qualitaetsstufe: {value}.");
        return new GpsQualityGrade(value);
    }
}
