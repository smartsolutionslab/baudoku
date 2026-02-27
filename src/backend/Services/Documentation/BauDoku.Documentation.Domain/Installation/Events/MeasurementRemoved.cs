using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record MeasurementRemoved(
    Guid InstallationId,
    Guid MeasurementId,
    DateTime OccurredOn) : IDomainEvent;
