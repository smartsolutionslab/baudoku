namespace BauDoku.Documentation.Application.Contracts;

public sealed record BoundingBox(
    double MinLatitude,
    double MinLongitude,
    double MaxLatitude,
    double MaxLongitude);
