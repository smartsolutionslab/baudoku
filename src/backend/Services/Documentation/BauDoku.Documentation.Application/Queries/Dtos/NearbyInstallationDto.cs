namespace BauDoku.Documentation.Application.Queries.Dtos;

public sealed record NearbyInstallationDto(
    Guid Id,
    Guid ProjectId,
    string Type,
    string Status,
    double Latitude,
    double Longitude,
    string? Description,
    DateTime CreatedAt,
    int PhotoCount,
    int MeasurementCount,
    double DistanceMeters);
