namespace SmartSolutionsLab.BauDoku.Documentation.Api.Endpoints;

public sealed record GpsPositionRequest(
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
