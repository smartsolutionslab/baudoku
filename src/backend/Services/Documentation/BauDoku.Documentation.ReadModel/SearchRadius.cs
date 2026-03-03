using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.ReadModel;

public sealed record SearchRadius(
    Latitude Latitude,
    Longitude Longitude,
    RadiusMeters RadiusMeters);
