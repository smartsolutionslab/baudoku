using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Projects.Application.Commands.AddZone;

public sealed record AddZoneCommand(
    Guid ProjectId,
    string Name,
    string Type,
    Guid? ParentZoneId) : ICommand;
