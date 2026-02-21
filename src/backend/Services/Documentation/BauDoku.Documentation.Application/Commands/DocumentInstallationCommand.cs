using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands;

public sealed record DocumentInstallationCommand(
    ProjectIdentifier ProjectId,
    ZoneIdentifier? ZoneId,
    InstallationType Type,
    Latitude Latitude,
    Longitude Longitude,
    double? Altitude,
    HorizontalAccuracy HorizontalAccuracy,
    GpsSource GpsSource,
    CorrectionService? CorrectionService,
    RtkFixStatus? RtkFixStatus,
    int? SatelliteCount,
    double? Hdop,
    double? CorrectionAge,
    Description? Description,
    CableType? CableType,
    CrossSection? CrossSection,
    CableColor? CableColor,
    int? ConductorCount,
    int? DepthMm,
    Manufacturer? Manufacturer,
    ModelName? ModelName,
    SerialNumber? SerialNumber) : ICommand<InstallationIdentifier>;
