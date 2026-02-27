using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationCableSpecUpdated(
    Guid InstallationId,
    string? CableType,
    decimal? CrossSection,
    string? CableColor,
    int? ConductorCount,
    DateTime OccurredOn) : IDomainEvent;
