using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record MeasurementRecorded(
    InstallationId InstallationId,
    MeasurementId MeasurementId,
    DateTime OccurredOn) : IDomainEvent;
