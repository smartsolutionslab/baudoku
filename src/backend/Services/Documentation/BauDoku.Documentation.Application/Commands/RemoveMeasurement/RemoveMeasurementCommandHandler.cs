using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RemoveMeasurement;

public sealed class RemoveMeasurementCommandHandler(IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveMeasurementCommand>
{
    public async Task Handle(RemoveMeasurementCommand command, CancellationToken cancellationToken)
    {
        var installationId = InstallationIdentifier.From(command.InstallationId);
        var installation = await installations.GetByIdAsync(installationId, cancellationToken) ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var measurementId = MeasurementIdentifier.From(command.MeasurementId);
        installation.RemoveMeasurement(measurementId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.MeasurementsRemoved.Add(1);
    }
}
