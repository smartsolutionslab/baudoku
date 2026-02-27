using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDeviceInfoUpdated(
    Guid InstallationId,
    string? Manufacturer,
    string? ModelName,
    string? SerialNumber,
    DateTime OccurredOn) : IDomainEvent;
