using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Application.Diagnostics;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;

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
