using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDescriptionUpdated(
    Guid InstallationId,
    string? Description,
    DateTime OccurredOn) : IDomainEvent;
