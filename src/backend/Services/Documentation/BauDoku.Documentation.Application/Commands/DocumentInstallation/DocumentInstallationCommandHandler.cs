using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.DocumentInstallation;

public sealed class DocumentInstallationCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<DocumentInstallationCommand, Guid>
{
    public async Task<Guid> Handle(DocumentInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, type,
            latitude, longitude, altitude,
            horizontalAccuracy, gpsSource,
            correctionService, rtkFixStatus,
            satelliteCount, hdop, correctionAge,
            description,
            cableType, crossSection, cableColor, conductorCount, depthMm,
            manufacturer, modelName, serialNumber) = command;

        var installationId = InstallationIdentifier.New();

        var position = GpsPosition.Create(
            latitude, longitude, altitude,
            horizontalAccuracy, gpsSource,
            correctionService, rtkFixStatus,
            satelliteCount, hdop, correctionAge);

        var installation = Installation.Create(
            installationId,
            projectId,
            zoneId,
            type,
            position,
            description,
            cableType is not null ? CableSpec.Create(cableType, crossSection, cableColor, conductorCount) : null,
            Depth.FromNullable(depthMm),
            manufacturer,
            modelName,
            serialNumber);

        await installations.AddAsync(installation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsDocumented.Add(1);
        DocumentationMetrics.GpsHorizontalAccuracy.Record(horizontalAccuracy.Value);

        return installationId.Value;
    }
}
