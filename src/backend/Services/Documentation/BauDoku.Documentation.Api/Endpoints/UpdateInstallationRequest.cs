namespace BauDoku.Documentation.Api.Endpoints;

public sealed record UpdateInstallationRequest(
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
    string? Description,
    string? CableType,
    decimal? CrossSection,
    string? CableColor,
    int? ConductorCount,
    int? DepthMm);
