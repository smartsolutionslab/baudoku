using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Queries;

public sealed record GetChangesSinceQuery(
    DeviceIdentifier DeviceId,
    SyncLimit Limit,
    DateTime? Since = null) : IQuery<ChangeSetResult>;
