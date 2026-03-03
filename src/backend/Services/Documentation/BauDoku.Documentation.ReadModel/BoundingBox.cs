using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.ReadModel;

public sealed record BoundingBox(
    Latitude MinLatitude,
    Longitude MinLongitude,
    Latitude MaxLatitude,
    Longitude MaxLongitude);
