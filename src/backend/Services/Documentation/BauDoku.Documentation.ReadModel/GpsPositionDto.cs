namespace BauDoku.Documentation.ReadModel;

public sealed record GpsPositionDto(
    double Latitude,
    double Longitude,
    double? Altitude,
    double HorizontalAccuracy,
    string GpsSource,
    string? CorrectionService,
    string? RtkFixStatus,
    int? SatelliteCount,
    double? Hdop,
    double? CorrectionAge);
