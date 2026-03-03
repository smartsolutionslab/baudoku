using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record MeasurementRemoved(
    InstallationIdentifier InstallationId,
    MeasurementIdentifier MeasurementId,
    DateTime OccurredOn) : IDomainEvent;
