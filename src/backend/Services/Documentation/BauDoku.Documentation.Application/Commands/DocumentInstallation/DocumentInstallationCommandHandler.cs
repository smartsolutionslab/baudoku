using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.DocumentInstallation;

public sealed class DocumentInstallationCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<DocumentInstallationCommand, Guid>
{
    public async Task<Guid> Handle(DocumentInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, type, latitude, longitude, altitude, horizontalAccuracy, gpsSource,
             correctionService, rtkFixStatus, satelliteCount, hdop, correctionAge, description,
             cableType, crossSection, cableColor, conductorCount, depthMm, manufacturer, modelName, serialNumber) = command;

        var installationId = InstallationIdentifier.New();

        var position = GpsPosition.Create(
            latitude, longitude, altitude, horizontalAccuracy, gpsSource,
            correctionService, rtkFixStatus, satelliteCount, hdop, correctionAge);

        var installation = Installation.Create(
            installationId,
            ProjectIdentifier.From(projectId),
            zoneId is not null ? ZoneIdentifier.From(zoneId.Value) : null,
            InstallationType.From(type),
            position,
            description is not null ? Description.From(description) : null,
            cableType is not null ? CableSpec.Create(cableType, crossSection, cableColor, conductorCount) : null,
            depthMm is not null ? Depth.From(depthMm.Value) : null,
            manufacturer is not null ? Manufacturer.From(manufacturer) : null,
            modelName is not null ? ModelName.From(modelName) : null,
            serialNumber is not null ? SerialNumber.From(serialNumber) : null);

        await installations.AddAsync(installation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsDocumented.Add(1);
        DocumentationMetrics.GpsHorizontalAccuracy.Record(horizontalAccuracy);

        return installationId.Value;
    }
}
