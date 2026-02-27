using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record PhotoAdded(
    Guid InstallationId,
    Guid PhotoId,
    string FileName,
    string BlobUrl,
    string ContentType,
    long FileSize,
    string PhotoType,
    string? Caption,
    string? Description,
    double? Latitude,
    double? Longitude,
    double? Altitude,
    double? HorizontalAccuracy,
    string? GpsSource,
    string? CorrectionService,
    string? RtkFixStatus,
    int? SatelliteCount,
    double? Hdop,
    double? CorrectionAge,
    DateTime TakenAt,
    DateTime OccurredOn) : IDomainEvent;
