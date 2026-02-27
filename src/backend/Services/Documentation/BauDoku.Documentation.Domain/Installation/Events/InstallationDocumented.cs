using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDocumented(
    Guid InstallationId,
    Guid ProjectId,
    Guid? ZoneId,
    string Type,
    string Status,
    double Latitude,
    double Longitude,
    double? Altitude,
    double HorizontalAccuracy,
    string GpsSource,
    string? CorrectionService,
    string? RtkFixStatus,
    int? SatelliteCount,
    double? Hdop,
    double? CorrectionAge,
    string QualityGrade,
    string? Description,
    string? CableType,
    decimal? CrossSection,
    string? CableColor,
    int? ConductorCount,
    int? DepthMm,
    string? Manufacturer,
    string? ModelName,
    string? SerialNumber,
    DateTime CreatedAt,
    DateTime OccurredOn) : IDomainEvent;
