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
            Latitude.From(latitude), Longitude.From(longitude), altitude,
            HorizontalAccuracy.From(horizontalAccuracy), GpsSource.From(gpsSource),
            CorrectionService.FromNullable(correctionService),
            RtkFixStatus.FromNullable(rtkFixStatus),
            satelliteCount, hdop, correctionAge);

        var installation = Installation.Create(
            installationId,
            ProjectIdentifier.From(projectId),
            ZoneIdentifier.FromNullable(zoneId),
            InstallationType.From(type),
            position,
            Description.FromNullable(description),
            cableType is not null ? CableSpec.Create(cableType, crossSection, cableColor, conductorCount) : null,
            Depth.FromNullable(depthMm),
            Manufacturer.FromNullable(manufacturer),
            ModelName.FromNullable(modelName),
            SerialNumber.FromNullable(serialNumber));

        await installations.AddAsync(installation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsDocumented.Add(1);
        DocumentationMetrics.GpsHorizontalAccuracy.Record(horizontalAccuracy);

        return installationId.Value;
    }
}
