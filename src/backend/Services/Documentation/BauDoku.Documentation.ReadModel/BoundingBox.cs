using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.ReadModel;

public sealed record BoundingBox(
    Latitude MinLatitude,
    Longitude MinLongitude,
    Latitude MaxLatitude,
    Longitude MaxLongitude);
