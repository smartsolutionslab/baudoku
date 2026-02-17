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
        var installationId = InstallationIdentifier.New();

        var projectId = ProjectIdentifier.From(command.ProjectId);
        var zoneId = command.ZoneId is not null ? ZoneIdentifier.From(command.ZoneId.Value) : null;

        var position = GpsPosition.Create(
            command.Latitude,
            command.Longitude,
            command.Altitude,
            command.HorizontalAccuracy,
            command.GpsSource,
            command.CorrectionService,
            command.RtkFixStatus,
            command.SatelliteCount,
            command.Hdop,
            command.CorrectionAge);

        var description = command.Description is not null ? Description.From(command.Description) : null;

        var cableSpec = command.CableType is not null ? CableSpec.Create(command.CableType, command.CrossSection, command.CableColor, command.ConductorCount) : null;

        var depth = command.DepthMm is not null ? Depth.From(command.DepthMm.Value) : null;

        var manufacturer = command.Manufacturer is not null ? Manufacturer.From(command.Manufacturer) : null;

        var modelName = command.ModelName is not null ? ModelName.From(command.ModelName) : null;

        var serialNumber = command.SerialNumber is not null ? SerialNumber.From(command.SerialNumber) : null;

        var installation = Installation.Create(
            installationId,
            projectId,
            zoneId,
            InstallationType.From(command.Type),
            position,
            description,
            cableSpec,
            depth,
            manufacturer,
            modelName,
            serialNumber);

        await installations.AddAsync(installation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsDocumented.Add(1);
        DocumentationMetrics.GpsHorizontalAccuracy.Record(command.HorizontalAccuracy);

        return installationId.Value;
    }
}
