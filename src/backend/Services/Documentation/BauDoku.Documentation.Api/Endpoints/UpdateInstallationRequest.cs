namespace SmartSolutionsLab.BauDoku.Documentation.Api.Endpoints;

public sealed record UpdateInstallationRequest(
    GpsPositionRequest? Position,
    string? Description,
    string? CableType,
    decimal? CrossSection,
    string? CableColor,
    int? ConductorCount,
    int? DepthMm,
    string? Manufacturer,
    string? ModelName,
    string? SerialNumber);
