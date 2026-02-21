using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries;

namespace BauDoku.Sync.Application.Queries.Handlers;

public sealed class GetChangesSinceQueryHandler(IEntityVersionReadStore entityVersionReadStore)
    : IQueryHandler<GetChangesSinceQuery, ChangeSetResult>
{
    public async Task<ChangeSetResult> Handle(GetChangesSinceQuery query, CancellationToken cancellationToken = default)
    {
        var limit = query.Limit ?? 100;
        var requestedLimit = limit + 1;

        var changes = await entityVersionReadStore.GetChangedSinceAsync(
            query.Since, query.DeviceId, requestedLimit, cancellationToken);

        var hasMore = changes.Count > limit;
        if (hasMore)
            changes = changes.Take(limit).ToList();

        return new ChangeSetResult(changes, DateTime.UtcNow, hasMore);
    }
}
