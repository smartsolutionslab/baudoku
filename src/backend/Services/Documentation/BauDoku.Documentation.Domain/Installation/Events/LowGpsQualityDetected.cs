using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record LowGpsQualityDetected(
    InstallationIdentifier InstallationId,
    GpsQualityGrade QualityGrade,
    DateTime OccurredOn) : IDomainEvent;
