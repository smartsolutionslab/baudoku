using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Queries;

public sealed record GetChangesSinceQuery(
    DeviceIdentifier DeviceId,
    DateTime? Since,
    int? Limit) : IQuery<ChangeSetResult>;
