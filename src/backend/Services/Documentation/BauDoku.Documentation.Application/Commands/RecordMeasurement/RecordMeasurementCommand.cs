using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.RecordMeasurement;

public sealed record RecordMeasurementCommand(
    Guid InstallationId,
    string Type,
    double Value,
    string Unit,
    double? MinThreshold,
    double? MaxThreshold,
    string? Notes) : ICommand<Guid>;
