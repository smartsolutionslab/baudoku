using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Commands.DeleteProject;

public sealed record DeleteProjectCommand(ProjectIdentifier ProjectId) : ICommand;
