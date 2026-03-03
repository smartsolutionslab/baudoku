namespace BauDoku.Documentation.ReadModel;

public sealed record NearbyInstallationDto(
    Guid Id,
    Guid ProjectId,
    string Type,
    string Status,
    string QualityGrade,
    double Latitude,
    double Longitude,
    string? Description,
    DateTime CreatedAt,
    int PhotoCount,
    int MeasurementCount,
    double DistanceMeters);
