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
        var installation = await installations.GetByIdAsync(command.InstallationId, cancellationToken);

        installation.Delete();
        installations.Remove(installation);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsDeleted.Add(1);
    }
}
