using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class CompleteInstallationCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<CompleteInstallationCommand>
{
    public async Task Handle(CompleteInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var installationId = command.InstallationId;

        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        installation.MarkAsCompleted();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsCompleted.Add(1);
    }
}
