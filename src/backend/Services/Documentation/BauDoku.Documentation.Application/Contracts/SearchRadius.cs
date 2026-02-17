namespace BauDoku.Documentation.Application.Contracts;

public sealed record SearchRadius(
    double Latitude,
    double Longitude,
    double RadiusMeters);
