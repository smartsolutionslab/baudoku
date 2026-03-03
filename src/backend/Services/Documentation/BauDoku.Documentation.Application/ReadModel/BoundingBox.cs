using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.ReadModel;

public sealed record BoundingBox(
    Latitude MinLatitude,
    Longitude MinLongitude,
    Latitude MaxLatitude,
    Longitude MaxLongitude);
