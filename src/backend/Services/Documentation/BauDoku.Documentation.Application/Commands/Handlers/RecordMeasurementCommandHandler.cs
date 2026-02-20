using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class RecordMeasurementCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<RecordMeasurementCommand, MeasurementIdentifier>
{
    public async Task<MeasurementIdentifier> Handle(RecordMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, type, value, unit, minThreshold, maxThreshold, notes) = command;

        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        var measurementId = MeasurementIdentifier.New();
        var measurementValue = MeasurementValue.Create(value, unit.Value, minThreshold, maxThreshold);
        var notesVo = Notes.FromNullable(notes);

        installation.RecordMeasurement(measurementId, type, measurementValue, notesVo);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.MeasurementsRecorded.Add(1);

        return measurementId;
    }
}
