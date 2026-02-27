using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record MeasurementRecorded(
    Guid InstallationId,
    Guid MeasurementId,
    string Type,
    double Value,
    string Unit,
    double? MinThreshold,
    double? MaxThreshold,
    string Result,
    string? Notes,
    DateTime MeasuredAt,
    DateTime OccurredOn) : IDomainEvent;
