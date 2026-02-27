using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationPositionUpdated(
    Guid InstallationId,
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
    DateTime OccurredOn) : IDomainEvent;
