using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Queries.GetConflicts;

public sealed class GetConflictsQueryHandler : IQueryHandler<GetConflictsQuery, List<ConflictDto>>
{
    private readonly ISyncBatchReadRepository _readRepository;

    public GetConflictsQueryHandler(ISyncBatchReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<List<ConflictDto>> Handle(
        GetConflictsQuery query,
        CancellationToken cancellationToken = default)
    {
        var deviceId = query.DeviceId is not null ? new DeviceId(query.DeviceId) : null;
        var status = query.Status is not null ? new ConflictStatus(query.Status) : null;

        return await _readRepository.GetConflictsAsync(deviceId, status, cancellationToken);
    }
}
