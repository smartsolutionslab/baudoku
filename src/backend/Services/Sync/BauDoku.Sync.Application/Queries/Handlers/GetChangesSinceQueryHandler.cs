using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Sync.Application.Contracts;
using SmartSolutionsLab.BauDoku.Sync.Application.Queries;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Queries.Handlers;

public sealed class GetChangesSinceQueryHandler(IEntityVersionReadStore entityVersionReadStore)
    : IQueryHandler<GetChangesSinceQuery, ChangeSetResult>
{
    public async Task<ChangeSetResult> Handle(GetChangesSinceQuery query, CancellationToken cancellationToken = default)
    {
        var (deviceId, syncLimit, since) = query;

        var limit = syncLimit.Value;
        var requestedLimit = limit + 1;

        var changes = await entityVersionReadStore.GetChangedSinceAsync(
            since, deviceId, requestedLimit, cancellationToken);

        var hasMore = changes.Count > limit;
        if (hasMore)
            changes = changes.Take(limit).ToList();

        return new ChangeSetResult(changes, DateTime.UtcNow, hasMore);
    }
}
