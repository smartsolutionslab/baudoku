using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDeviceInfoUpdated(
    InstallationIdentifier InstallationId,
    Manufacturer? Manufacturer,
    ModelName? ModelName,
    SerialNumber? SerialNumber,
    DateTime OccurredOn) : IDomainEvent;
