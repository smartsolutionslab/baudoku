using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Commands;

public sealed record DeleteProjectCommand(ProjectIdentifier ProjectId) : ICommand;
