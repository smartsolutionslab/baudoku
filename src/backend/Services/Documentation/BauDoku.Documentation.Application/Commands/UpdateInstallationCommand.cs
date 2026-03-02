using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands;

public sealed record UpdateInstallationCommand(
    InstallationIdentifier InstallationId,
    GpsPosition? Position,
    Description? Description,
    CableType? CableType,
    CrossSection? CrossSection,
    CableColor? CableColor,
    int? ConductorCount,
    int? DepthMm,
    Manufacturer? Manufacturer,
    ModelName? ModelName,
    SerialNumber? SerialNumber) : ICommand;
