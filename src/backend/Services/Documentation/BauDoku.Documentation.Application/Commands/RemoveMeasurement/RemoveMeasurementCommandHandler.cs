using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RemoveMeasurement;

public sealed class RemoveMeasurementCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveMeasurementCommand>
{
    public async Task Handle(RemoveMeasurementCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, measurementId) = command;
        var installation = await installations.GetByIdAsync(
            InstallationIdentifier.From(installationId), cancellationToken);

        installation.RemoveMeasurement(MeasurementIdentifier.From(measurementId));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.MeasurementsRemoved.Add(1);
    }
}
