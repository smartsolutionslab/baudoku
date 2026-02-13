using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.DocumentInstallation;

public sealed class DocumentInstallationCommandHandler
    : ICommandHandler<DocumentInstallationCommand, Guid>
{
    private readonly IInstallationRepository _installationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentInstallationCommandHandler(
        IInstallationRepository installationRepository,
        IUnitOfWork unitOfWork)
    {
        _installationRepository = installationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DocumentInstallationCommand command, CancellationToken cancellationToken)
    {
        var installationId = InstallationId.New();

        var position = new GpsPosition(
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

        var description = command.Description is not null
            ? new Description(command.Description) : null;

        var cableSpec = command.CableType is not null
            ? new CableSpec(command.CableType, command.CrossSection, command.CableColor, command.ConductorCount) : null;

        var depth = command.DepthMm is not null
            ? new Depth(command.DepthMm.Value) : null;

        var manufacturer = command.Manufacturer is not null
            ? new Manufacturer(command.Manufacturer) : null;

        var modelName = command.ModelName is not null
            ? new ModelName(command.ModelName) : null;

        var serialNumber = command.SerialNumber is not null
            ? new SerialNumber(command.SerialNumber) : null;

        var installation = Installation.Create(
            installationId,
            command.ProjectId,
            command.ZoneId,
            new InstallationType(command.Type),
            position,
            description,
            cableSpec,
            depth,
            manufacturer,
            modelName,
            serialNumber);

        await _installationRepository.AddAsync(installation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.InstallationsDocumented.Add(1);
        DocumentationMetrics.GpsHorizontalAccuracy.Record(command.HorizontalAccuracy);

        return installationId.Value;
    }
}
