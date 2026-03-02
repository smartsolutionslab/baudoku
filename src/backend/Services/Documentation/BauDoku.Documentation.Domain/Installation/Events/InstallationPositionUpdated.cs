using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationPositionUpdated(
    InstallationIdentifier InstallationId,
    Latitude Latitude,
    Longitude Longitude,
    Altitude? Altitude,
    HorizontalAccuracy HorizontalAccuracy,
    GpsSource GpsSource,
    CorrectionService? CorrectionService,
    RtkFixStatus? RtkFixStatus,
    SatelliteCount? SatelliteCount,
    Hdop? Hdop,
    CorrectionAge? CorrectionAge,
    GpsQualityGrade QualityGrade,
    DateTime OccurredOn) : IDomainEvent;
