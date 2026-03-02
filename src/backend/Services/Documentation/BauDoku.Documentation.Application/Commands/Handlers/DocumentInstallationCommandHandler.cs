using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class DocumentInstallationCommandHandler(IInstallationRepository installations): ICommandHandler<DocumentInstallationCommand, InstallationIdentifier>
{
    public async Task<InstallationIdentifier> Handle(DocumentInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var installationId = InstallationIdentifier.New();

        var installation = Installation.Create(
            installationId,
            command.ProjectId,
            command.ZoneId,
            command.Type,
            command.Position,
            command.Description,
            command.CableType is not null ? CableSpec.Create(command.CableType, command.CrossSection, command.CableColor, command.ConductorCount) : null,
            Depth.FromNullable(command.DepthMm),
            command.Manufacturer,
            command.ModelName,
            command.SerialNumber);

        await installations.SaveAsync(installation, cancellationToken);

        DocumentationMetrics.InstallationsDocumented.Add(1);
        DocumentationMetrics.GpsHorizontalAccuracy.Record(command.Position.HorizontalAccuracy.Value);

        return installationId;
    }
}
