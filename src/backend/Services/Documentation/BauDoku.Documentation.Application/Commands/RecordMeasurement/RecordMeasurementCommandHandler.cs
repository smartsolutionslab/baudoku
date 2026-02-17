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
        var installationId = InstallationIdentifier.From(command.InstallationId);
        var installation = await installations.GetByIdAsync(installationId, cancellationToken) ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var measurementId = MeasurementIdentifier.New();
        var type = MeasurementType.From(command.Type);
        var value = MeasurementValue.Create(command.Value, command.Unit, command.MinThreshold, command.MaxThreshold);

        installation.RecordMeasurement(measurementId, type, value, command.Notes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.MeasurementsRecorded.Add(1);

        return measurementId.Value;
    }
}
