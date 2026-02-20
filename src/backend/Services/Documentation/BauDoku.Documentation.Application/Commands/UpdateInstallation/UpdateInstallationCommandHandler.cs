using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.UpdateInstallation;

public sealed class UpdateInstallationCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateInstallationCommand>
{
    public async Task Handle(UpdateInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId,
            latitude, longitude, altitude,
            horizontalAccuracy, gpsSource,
            correctionService, rtkFixStatus,
            satelliteCount, hdop, correctionAge,
            description,
            cableType, crossSection, cableColor, conductorCount, depthMm,
            manufacturer, modelName, serialNumber) = command;

        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        if (latitude is not null && longitude is not null && horizontalAccuracy is not null && gpsSource is not null)
        {
            var position = GpsPosition.Create(
                latitude, longitude, altitude,
                horizontalAccuracy, gpsSource,
                correctionService, rtkFixStatus,
                satelliteCount, hdop, correctionAge);

            installation.UpdatePosition(position);
        }

        if (description is not null)
        {
            installation.UpdateDescription(description);
        }

        if (cableType is not null)
        {
            installation.UpdateCableSpec(CableSpec.Create(cableType, crossSection, cableColor, conductorCount));
        }

        if (depthMm.HasValue)
        {
            installation.UpdateDepth(Depth.From(depthMm.Value));
        }

        if (manufacturer is not null || modelName is not null || serialNumber is not null)
        {
            installation.UpdateDeviceInfo(manufacturer, modelName, serialNumber);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
