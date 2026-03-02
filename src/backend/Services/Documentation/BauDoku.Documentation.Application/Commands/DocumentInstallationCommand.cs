using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands;

public sealed record DocumentInstallationCommand(
    ProjectIdentifier ProjectId,
    ZoneIdentifier? ZoneId,
    InstallationType Type,
    GpsPosition Position,
    Description? Description,
    CableType? CableType,
    CrossSection? CrossSection,
    CableColor? CableColor,
    ConductorCount? ConductorCount,
    Depth? Depth,
    Manufacturer? Manufacturer,
    ModelName? ModelName,
    SerialNumber? SerialNumber) : ICommand<InstallationIdentifier>;
