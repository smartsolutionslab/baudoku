using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class RemoveMeasurementCommandHandler(IInstallationRepository installations): ICommandHandler<RemoveMeasurementCommand>
{
    public async Task Handle(RemoveMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, measurementId) = command;

        var installation = await installations.With(installationId, cancellationToken);
        installation.RemoveMeasurement(measurementId);
        await installations.SaveAsync(installation, cancellationToken);

        DocumentationMetrics.MeasurementsRemoved.Add(1);
    }
}
