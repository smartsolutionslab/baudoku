using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.RecordMeasurement;

public sealed record RecordMeasurementCommand(
    InstallationIdentifier InstallationId,
    MeasurementType Type,
    double Value,
    MeasurementUnit Unit,
    double? MinThreshold,
    double? MaxThreshold,
    string? Notes) : ICommand<Guid>;
