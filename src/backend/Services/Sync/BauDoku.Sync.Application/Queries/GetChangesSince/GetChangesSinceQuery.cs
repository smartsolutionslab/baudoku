using BauDoku.BuildingBlocks.Application.Queries;

namespace BauDoku.Sync.Application.Queries.GetChangesSince;

public sealed record GetChangesSinceQuery(
    string DeviceId,
    DateTime? Since,
    int? Limit) : IQuery<ChangeSetResult>;
