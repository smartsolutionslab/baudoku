using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.RemoveMeasurement;

public sealed record RemoveMeasurementCommand(
    Guid InstallationId,
    Guid MeasurementId) : ICommand;
