using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Contracts;

public sealed record BoundingBox(
    Latitude MinLatitude,
    Longitude MinLongitude,
    Latitude MaxLatitude,
    Longitude MaxLongitude);
