using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record MeasurementRecorded(
    InstallationIdentifier InstallationId,
    MeasurementIdentifier MeasurementId,
    MeasurementType Type,
    double Value,
    MeasurementUnit Unit,
    double? MinThreshold,
    double? MaxThreshold,
    MeasurementResult Result,
    Notes? Notes,
    DateTime MeasuredAt,
    DateTime OccurredOn) : IDomainEvent;
