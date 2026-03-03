using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Application.Diagnostics;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;

public sealed class DocumentInstallationCommandHandler(IInstallationRepository installations): ICommandHandler<DocumentInstallationCommand, InstallationIdentifier>
{
    public async Task<InstallationIdentifier> Handle(DocumentInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, installationType, position, description, cableType, crossSection, cableColor, conductorCount, depth, manufacturer, modelName, serialNumber) = command;

        var installationId = InstallationIdentifier.New();

        var installation = Installation.Create(
            installationId,
            projectId,
            zoneId,
            installationType,
            position,
            description,
            CableSpec.FromNullable(cableType, crossSection, cableColor, conductorCount),
            depth,
            manufacturer,
            modelName,
            serialNumber);

        await installations.SaveAsync(installation, cancellationToken);

        DocumentationMetrics.InstallationsDocumented.Add(1);
        DocumentationMetrics.GpsHorizontalAccuracy.Record(command.Position.HorizontalAccuracy.Value);

        return installationId;
    }
}
