using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Application.Diagnostics;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;

public sealed class DeleteInstallationCommandHandler(IInstallationRepository installations) : ICommandHandler<DeleteInstallationCommand>
{
    public async Task Handle(DeleteInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var installationId = command.InstallationId;

        var installation = await installations.With(installationId, cancellationToken);
        installation.Delete();
        await installations.SaveAsync(installation, cancellationToken);

        DocumentationMetrics.InstallationsDeleted.Add(1);
    }
}
