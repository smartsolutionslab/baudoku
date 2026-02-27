using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class DeleteInstallationCommandHandler(IInstallationRepository installations) : ICommandHandler<DeleteInstallationCommand>
{
    public async Task Handle(DeleteInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var installationId = command.InstallationId;

        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        installation.Delete();

        await installations.SaveAsync(installation, cancellationToken);

        DocumentationMetrics.InstallationsDeleted.Add(1);
    }
}
