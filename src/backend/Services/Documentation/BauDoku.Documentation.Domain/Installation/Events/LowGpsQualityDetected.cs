using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record LowGpsQualityDetected(
    Guid InstallationId,
    string QualityGrade,
    DateTime OccurredOn) : IDomainEvent;
