using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.DocumentInstallation;

public sealed record DocumentInstallationCommand(
    Guid ProjectId,
    Guid? ZoneId,
    string Type,
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
    string? Description,
    string? CableType,
    int? CrossSection,
    string? CableColor,
    int? ConductorCount,
    int? DepthMm,
    string? Manufacturer,
    string? ModelName,
    string? SerialNumber) : ICommand<Guid>;
