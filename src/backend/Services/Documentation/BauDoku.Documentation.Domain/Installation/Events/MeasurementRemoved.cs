using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record MeasurementRemoved(
    InstallationIdentifier InstallationIdentifier,
    MeasurementIdentifier MeasurementIdentifier,
    DateTime OccurredOn) : IDomainEvent;
