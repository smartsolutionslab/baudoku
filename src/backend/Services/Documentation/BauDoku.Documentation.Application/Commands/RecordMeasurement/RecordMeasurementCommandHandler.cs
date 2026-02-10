using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RecordMeasurement;

public sealed class RecordMeasurementCommandHandler : ICommandHandler<RecordMeasurementCommand, Guid>
{
    private readonly IInstallationRepository _installationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordMeasurementCommandHandler(
        IInstallationRepository installationRepository,
        IUnitOfWork unitOfWork)
    {
        _installationRepository = installationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RecordMeasurementCommand command, CancellationToken cancellationToken)
    {
        var installationId = new InstallationId(command.InstallationId);
        var installation = await _installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var measurementId = MeasurementId.New();
        var type = new MeasurementType(command.Type);
        var value = new MeasurementValue(command.Value, command.Unit, command.MinThreshold, command.MaxThreshold);

        installation.RecordMeasurement(measurementId, type, value, command.Notes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return measurementId.Value;
    }
}
