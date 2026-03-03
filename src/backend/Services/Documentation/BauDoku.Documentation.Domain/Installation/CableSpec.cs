using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record CableSpec : IValueObject
{
    public CableType CableType { get; }
    public CrossSection? CrossSection { get; }
    public CableColor? Color { get; }
    public ConductorCount? ConductorCount { get; }

    private CableSpec(CableType cableType, CrossSection? crossSection, CableColor? color, ConductorCount? conductorCount)
    {
        CableType = cableType;
        CrossSection = crossSection;
        Color = color;
        ConductorCount = conductorCount;
    }

    public static CableSpec Create(CableType cableType, CrossSection? crossSection = null, CableColor? color = null, ConductorCount? conductorCount = null)
    {
        return new CableSpec(cableType, crossSection, color, conductorCount);
    }

    public static CableSpec? FromNullable(CableType? cableType, CrossSection? crossSection = null, CableColor? color = null, ConductorCount? conductorCount = null)
        => cableType is not null ? new CableSpec(cableType, crossSection, color, conductorCount) : null;
}
