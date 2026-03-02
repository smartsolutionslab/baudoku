using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Contracts;

public sealed record SearchRadius(
    Latitude Latitude,
    Longitude Longitude,
    RadiusMeters RadiusMeters);
