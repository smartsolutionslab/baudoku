using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.UpdateInstallation;

public sealed class UpdateInstallationCommandHandler : ICommandHandler<UpdateInstallationCommand>
{
    private readonly IInstallationRepository installationRepository;
    private readonly IUnitOfWork unitOfWork;

    public UpdateInstallationCommandHandler(
        IInstallationRepository installationRepository,
        IUnitOfWork unitOfWork)
    {
        this.installationRepository = installationRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateInstallationCommand command, CancellationToken cancellationToken)
    {
        var installationId = InstallationIdentifier.From(command.InstallationId);
        var installation = await installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        if (command.Latitude.HasValue && command.Longitude.HasValue
            && command.HorizontalAccuracy.HasValue && command.GpsSource is not null)
        {
            var position = GpsPosition.Create(
                command.Latitude.Value,
                command.Longitude.Value,
                command.Altitude,
                command.HorizontalAccuracy.Value,
                command.GpsSource,
                command.CorrectionService,
                command.RtkFixStatus,
                command.SatelliteCount,
                command.Hdop,
                command.CorrectionAge);

            installation.UpdatePosition(position);
        }

        if (command.Description is not null)
        {
            installation.UpdateDescription(Description.From(command.Description));
        }

        if (command.CableType is not null)
        {
            installation.UpdateCableSpec(
                CableSpec.Create(command.CableType, command.CrossSection, command.CableColor, command.ConductorCount));
        }

        if (command.DepthMm.HasValue)
        {
            installation.UpdateDepth(Depth.From(command.DepthMm.Value));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
