using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.RecordMeasurement;

public sealed class RecordMeasurementCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<RecordMeasurementCommand, Guid>
{
    public async Task<Guid> Handle(RecordMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, type, value, unit, minThreshold, maxThreshold, notesText) = command;
        var installation = await installations.GetByIdAsync(
            InstallationIdentifier.From(installationId), cancellationToken);

        var measurementId = MeasurementIdentifier.New();
        var measurementType = MeasurementType.From(type);
        var measurementValue = MeasurementValue.Create(value, unit, minThreshold, maxThreshold);
        var notes = Notes.FromNullable(notesText);

        installation.RecordMeasurement(measurementId, measurementType, measurementValue, notes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.MeasurementsRecorded.Add(1);

        return measurementId.Value;
    }
}
