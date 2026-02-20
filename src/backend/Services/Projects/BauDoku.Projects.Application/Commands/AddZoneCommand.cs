using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Commands;

public sealed record AddZoneCommand(
    ProjectIdentifier ProjectId,
    ZoneName Name,
    ZoneType Type,
    ZoneIdentifier? ParentZoneId) : ICommand;
