using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record InstallationDocumented(
    InstallationIdentifier InstallationId,
    ProjectIdentifier ProjectId,
    ZoneIdentifier? ZoneId,
    InstallationType Type,
    InstallationStatus Status,
    Latitude? Latitude,
    Longitude? Longitude,
    Altitude? Altitude,
    HorizontalAccuracy? HorizontalAccuracy,
    GpsSource? GpsSource,
    CorrectionService? CorrectionService,
    RtkFixStatus? RtkFixStatus,
    SatelliteCount? SatelliteCount,
    Hdop? Hdop,
    CorrectionAge? CorrectionAge,
    GpsQualityGrade? QualityGrade,
    Description? Description,
    CableType? CableType,
    CrossSection? CrossSection,
    CableColor? CableColor,
    ConductorCount? ConductorCount,
    int? DepthMm,
    Manufacturer? Manufacturer,
    ModelName? ModelName,
    SerialNumber? SerialNumber,
    DateTime CreatedAt,
    DateTime OccurredOn) : IDomainEvent;
