using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.DeleteInstallation;

public sealed record DeleteInstallationCommand(Guid InstallationId) : ICommand;
