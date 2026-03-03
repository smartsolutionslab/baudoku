using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Application.Diagnostics;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;

public sealed class CompleteInstallationCommandHandler(IInstallationRepository installations) : ICommandHandler<CompleteInstallationCommand>
{
    public async Task Handle(CompleteInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var installationId = command.InstallationId;

        var installation = await installations.With(installationId, cancellationToken);
        installation.MarkAsCompleted();
        await installations.SaveAsync(installation, cancellationToken);

        DocumentationMetrics.InstallationsCompleted.Add(1);
    }
}
