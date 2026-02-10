using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record CableSpec : ValueObject
{
    public const int MaxCableTypeLength = 100;
    public const int MaxColorLength = 50;

    public string CableType { get; }
    public int? CrossSection { get; }
    public string? Color { get; }
    public int? ConductorCount { get; }

    public CableSpec(string cableType, int? crossSection = null, string? color = null, int? conductorCount = null)
    {
        if (string.IsNullOrWhiteSpace(cableType))
            throw new ArgumentException("Kabeltyp darf nicht leer sein.", nameof(cableType));
        if (cableType.Length > MaxCableTypeLength)
            throw new ArgumentException($"Kabeltyp darf max. {MaxCableTypeLength} Zeichen lang sein.", nameof(cableType));
        if (crossSection is <= 0)
            throw new ArgumentOutOfRangeException(nameof(crossSection), "Querschnitt muss groesser als 0 sein.");
        if (color is { Length: > MaxColorLength })
            throw new ArgumentException($"Farbe darf max. {MaxColorLength} Zeichen lang sein.", nameof(color));
        if (conductorCount is <= 0)
            throw new ArgumentOutOfRangeException(nameof(conductorCount), "Leiteranzahl muss groesser als 0 sein.");

        CableType = cableType;
        CrossSection = crossSection;
        Color = color;
        ConductorCount = conductorCount;
    }
}
