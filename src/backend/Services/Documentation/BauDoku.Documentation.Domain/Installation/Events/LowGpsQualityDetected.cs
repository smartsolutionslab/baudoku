using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record LowGpsQualityDetected(
    InstallationIdentifier InstallationIdentifier,
    GpsQualityGrade QualityGrade,
    DateTime OccurredOn) : IDomainEvent;
