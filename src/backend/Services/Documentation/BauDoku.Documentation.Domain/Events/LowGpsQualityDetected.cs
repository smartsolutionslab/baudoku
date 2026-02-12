using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record LowGpsQualityDetected(
    InstallationId InstallationId,
    GpsQualityGrade QualityGrade,
    DateTime OccurredOn) : IDomainEvent;
