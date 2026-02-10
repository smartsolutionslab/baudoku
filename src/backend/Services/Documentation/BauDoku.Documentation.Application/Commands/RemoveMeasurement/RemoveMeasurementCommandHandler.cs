using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RemoveMeasurement;

public sealed class RemoveMeasurementCommandHandler : ICommandHandler<RemoveMeasurementCommand>
{
    private readonly IInstallationRepository _installationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveMeasurementCommandHandler(
        IInstallationRepository installationRepository,
        IUnitOfWork unitOfWork)
    {
        _installationRepository = installationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveMeasurementCommand command, CancellationToken cancellationToken)
    {
        var installationId = new InstallationId(command.InstallationId);
        var installation = await _installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var measurementId = new MeasurementId(command.MeasurementId);
        installation.RemoveMeasurement(measurementId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
