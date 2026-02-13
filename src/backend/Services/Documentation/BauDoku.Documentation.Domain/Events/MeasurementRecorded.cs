using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record MeasurementRecorded(
    InstallationIdentifier InstallationIdentifier,
    MeasurementIdentifier MeasurementIdentifier,
    DateTime OccurredOn) : IDomainEvent;
