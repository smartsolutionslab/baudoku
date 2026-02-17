using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Projects.Application.Commands.DeleteProject;

public sealed record DeleteProjectCommand(Guid ProjectId) : ICommand;
