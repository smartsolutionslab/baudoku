using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Commands;

public sealed record DeleteProjectCommand(ProjectIdentifier ProjectId) : ICommand;
