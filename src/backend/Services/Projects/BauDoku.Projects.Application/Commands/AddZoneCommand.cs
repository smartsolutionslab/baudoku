using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Commands;

public sealed record AddZoneCommand(
    ProjectIdentifier ProjectId,
    ZoneName Name,
    ZoneType Type,
    ZoneIdentifier? ParentZoneId) : ICommand;
