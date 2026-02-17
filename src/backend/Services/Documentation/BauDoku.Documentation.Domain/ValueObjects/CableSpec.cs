using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record CableSpec : IValueObject
{
    public const int MaxCableTypeLength = 100;
    public const int MaxColorLength = 50;

    public string CableType { get; }
    public decimal? CrossSection { get; }
    public string? Color { get; }
    public int? ConductorCount { get; }

    private CableSpec(string cableType, decimal? crossSection, string? color, int? conductorCount)
    {
        CableType = cableType;
        CrossSection = crossSection;
        Color = color;
        ConductorCount = conductorCount;
    }

    public static CableSpec Create(string cableType, decimal? crossSection = null, string? color = null, int? conductorCount = null)
    {
        Ensure.That(cableType).IsNotNullOrWhiteSpace("Kabeltyp darf nicht leer sein.")
            .MaxLengthIs(MaxCableTypeLength, $"Kabeltyp darf max. {MaxCableTypeLength} Zeichen lang sein.");
        Ensure.That(crossSection).IsPositive("Querschnitt muss groesser als 0 sein.");
        Ensure.That(color).MaxLengthIs(MaxColorLength, $"Farbe darf max. {MaxColorLength} Zeichen lang sein.");
        Ensure.That(conductorCount).IsPositive("Leiteranzahl muss groesser als 0 sein.");
        return new CableSpec(cableType, crossSection, color, conductorCount);
    }
}
