using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record LowGpsQualityDetected(
    InstallationIdentifier InstallationId,
    GpsQualityGrade QualityGrade,
    DateTime OccurredOn) : IDomainEvent;
