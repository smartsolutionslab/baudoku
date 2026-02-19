using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.UpdateInstallation;

public sealed class UpdateInstallationCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateInstallationCommand>
{
    public async Task Handle(UpdateInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, latitude, longitude, altitude, horizontalAccuracy, gpsSource,
             correctionService, rtkFixStatus, satelliteCount, hdop, correctionAge, description,
             cableType, crossSection, cableColor, conductorCount, depthMm, manufacturer, modelName, serialNumber) = command;

        var installationIdentifier = InstallationIdentifier.From(installationId);
        var installation = await installations.GetByIdAsync(installationIdentifier, cancellationToken);

        if (latitude.HasValue && longitude.HasValue && horizontalAccuracy.HasValue && gpsSource is not null)
        {
            var position = GpsPosition.Create(
                Latitude.From(latitude.Value), Longitude.From(longitude.Value), altitude,
                HorizontalAccuracy.From(horizontalAccuracy.Value), GpsSource.From(gpsSource),
                CorrectionService.FromNullable(correctionService),
                RtkFixStatus.FromNullable(rtkFixStatus),
                satelliteCount, hdop, correctionAge);

            installation.UpdatePosition(position);
        }

        if (description is not null)
        {
            installation.UpdateDescription(Description.From(description));
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
            installation.UpdateDeviceInfo(
                Manufacturer.FromNullable(manufacturer),
                ModelName.FromNullable(modelName),
                SerialNumber.FromNullable(serialNumber));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
