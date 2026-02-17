using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.CompleteInstallation;

public sealed record CompleteInstallationCommand(Guid InstallationId) : ICommand;
