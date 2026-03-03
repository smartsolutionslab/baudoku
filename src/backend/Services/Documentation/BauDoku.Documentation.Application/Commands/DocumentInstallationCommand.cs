using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands;

public sealed record DocumentInstallationCommand(
    ProjectIdentifier ProjectId,
    ZoneIdentifier? ZoneId,
    InstallationType Type,
    GpsPosition? Position,
    Description? Description,
    CableType? CableType,
    CrossSection? CrossSection,
    CableColor? CableColor,
    ConductorCount? ConductorCount,
    Depth? Depth,
    Manufacturer? Manufacturer,
    ModelName? ModelName,
    SerialNumber? SerialNumber) : ICommand<InstallationIdentifier>;
