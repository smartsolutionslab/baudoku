using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.CompleteInstallation;

public sealed class CompleteInstallationCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<CompleteInstallationCommand>
{
    public async Task Handle(CompleteInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var installation = await installations.GetByIdAsync(command.InstallationId, cancellationToken);

        installation.MarkAsCompleted();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsCompleted.Add(1);
    }
}
