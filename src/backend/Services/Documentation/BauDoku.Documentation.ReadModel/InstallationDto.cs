namespace SmartSolutionsLab.BauDoku.Documentation.ReadModel;

public sealed record InstallationDto(
    Guid Id,
    Guid ProjectId,
    Guid? ZoneId,
    string Type,
    string Status,
    GpsPositionDto Position,
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
    DateTime? CompletedAt,
    IReadOnlyList<PhotoDto> Photos,
    IReadOnlyList<MeasurementDto> Measurements);
