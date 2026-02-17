using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RecordMeasurement;

public sealed class RecordMeasurementCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<RecordMeasurementCommand, Guid>
{
    public async Task<Guid> Handle(RecordMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, type, value, unit, minThreshold, maxThreshold, notes) = command;
        var installation = await installations.GetByIdAsync(
            InstallationIdentifier.From(installationId), cancellationToken)
            ?? throw new KeyNotFoundException($"Installation mit ID {installationId} nicht gefunden.");

        var measurementId = MeasurementIdentifier.New();
        var measurementType = MeasurementType.From(type);
        var measurementValue = MeasurementValue.Create(value, unit, minThreshold, maxThreshold);

        installation.RecordMeasurement(measurementId, measurementType, measurementValue, notes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.MeasurementsRecorded.Add(1);

        return measurementId.Value;
    }
}
