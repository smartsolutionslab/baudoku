using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record MeasurementRecorded(
    InstallationIdentifier InstallationIdentifier,
    MeasurementIdentifier MeasurementIdentifier,
    DateTime OccurredOn) : IDomainEvent;
