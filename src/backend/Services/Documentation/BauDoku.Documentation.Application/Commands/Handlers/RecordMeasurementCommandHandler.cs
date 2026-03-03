using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Application.Diagnostics;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;

public sealed class RecordMeasurementCommandHandler(IInstallationRepository installations): ICommandHandler<RecordMeasurementCommand, MeasurementIdentifier>
{
    public async Task<MeasurementIdentifier> Handle(RecordMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, type, value, unit, minThreshold, maxThreshold, notes) = command;

        var installation = await installations.With(installationId, cancellationToken);
        var measurementId = MeasurementIdentifier.New();
        var measurementValue = MeasurementValue.Create(value, unit, minThreshold, maxThreshold);
        var notesVo = notes;
        installation.RecordMeasurement(measurementId, type, measurementValue, notesVo);
        await installations.SaveAsync(installation, cancellationToken);

        DocumentationMetrics.MeasurementsRecorded.Add(1);

        return measurementId;
    }
}
