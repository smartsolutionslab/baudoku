using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Queries.GetChangesSince;

public sealed class GetChangesSinceQueryHandler(IEntityVersionReadStore entityVersionReadStore)
    : IQueryHandler<GetChangesSinceQuery, ChangeSetResult>
{
    public async Task<ChangeSetResult> Handle(GetChangesSinceQuery query, CancellationToken cancellationToken = default)
    {
        var (deviceId, since, queryLimit) = query;
        var limit = queryLimit ?? 100;
        var requestedLimit = limit + 1;

        var changes = await entityVersionReadStore.GetChangedSinceAsync(
            since, DeviceIdentifier.From(deviceId), requestedLimit, cancellationToken);

        var hasMore = changes.Count > limit;
        if (hasMore)
            changes = changes.Take(limit).ToList();

        return new ChangeSetResult(changes, DateTime.UtcNow, hasMore);
    }
}
