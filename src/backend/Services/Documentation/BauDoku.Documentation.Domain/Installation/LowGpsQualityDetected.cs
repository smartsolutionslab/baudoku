using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record LowGpsQualityDetected(
    InstallationIdentifier InstallationIdentifier,
    GpsQualityGrade QualityGrade,
    DateTime OccurredOn) : IDomainEvent;
