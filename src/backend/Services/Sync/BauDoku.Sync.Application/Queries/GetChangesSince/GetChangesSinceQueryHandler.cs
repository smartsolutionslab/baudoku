using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Queries.GetChangesSince;

public sealed class GetChangesSinceQueryHandler : IQueryHandler<GetChangesSinceQuery, ChangeSetResult>
{
    private readonly IEntityVersionReadStore entityVersionReadStore;

    public GetChangesSinceQueryHandler(IEntityVersionReadStore entityVersionReadStore)
    {
        this.entityVersionReadStore = entityVersionReadStore;
    }

    public async Task<ChangeSetResult> Handle(
        GetChangesSinceQuery query,
        CancellationToken cancellationToken = default)
    {
        var deviceId = DeviceIdentifier.From(query.DeviceId);
        var limit = query.Limit ?? 100;
        var requestedLimit = limit + 1;

        var changes = await entityVersionReadStore.GetChangedSinceAsync(
            query.Since, deviceId, requestedLimit, cancellationToken);

        var hasMore = changes.Count > limit;
        if (hasMore)
            changes = changes.Take(limit).ToList();

        return new ChangeSetResult(changes, DateTime.UtcNow, hasMore);
    }
}
