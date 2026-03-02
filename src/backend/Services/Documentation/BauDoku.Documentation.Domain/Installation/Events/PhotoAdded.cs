using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record PhotoAdded(
    InstallationIdentifier InstallationId,
    PhotoIdentifier PhotoId,
    FileName FileName,
    BlobUrl BlobUrl,
    ContentType ContentType,
    FileSize FileSize,
    PhotoType PhotoType,
    Caption? Caption,
    Description? Description,
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
    DateTime TakenAt,
    DateTime OccurredOn) : IDomainEvent;
