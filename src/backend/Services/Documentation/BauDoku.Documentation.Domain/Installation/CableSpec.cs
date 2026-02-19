using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record CableSpec : IValueObject
{
    public CableType CableType { get; }
    public CrossSection? CrossSection { get; }
    public CableColor? Color { get; }
    public int? ConductorCount { get; }

    private CableSpec(CableType cableType, CrossSection? crossSection, CableColor? color, int? conductorCount)
    {
        CableType = cableType;
        CrossSection = crossSection;
        Color = color;
        ConductorCount = conductorCount;
    }

    public static CableSpec Create(string cableType, decimal? crossSection = null, string? color = null, int? conductorCount = null)
    {
        Ensure.That(conductorCount).IsPositive("Leiteranzahl muss größer als 0 sein.");
        return new CableSpec(
            CableType.From(cableType),
            CrossSection.FromNullable(crossSection),
            CableColor.FromNullable(color),
            conductorCount);
    }
}
