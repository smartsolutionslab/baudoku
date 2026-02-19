using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.DeleteInstallation;

public sealed class DeleteInstallationCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteInstallationCommand>
{
    public async Task Handle(DeleteInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var installationId = InstallationIdentifier.From(command.InstallationId);
        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        installation.Delete();
        installations.Remove(installation);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsDeleted.Add(1);
    }
}
