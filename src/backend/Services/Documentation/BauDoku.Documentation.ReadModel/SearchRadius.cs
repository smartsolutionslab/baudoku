using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.ReadModel;

public sealed record SearchRadius(
    Latitude Latitude,
    Longitude Longitude,
    RadiusMeters RadiusMeters);
