using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record MeasurementRemoved(
    InstallationIdentifier InstallationId,
    MeasurementIdentifier MeasurementId,
    DateTime OccurredOn) : IDomainEvent;
