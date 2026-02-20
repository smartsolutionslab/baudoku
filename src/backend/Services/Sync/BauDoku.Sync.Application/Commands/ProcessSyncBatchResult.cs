using BauDoku.Sync.Application.Queries.Dtos;

namespace BauDoku.Sync.Application.Commands;

public sealed record ProcessSyncBatchResult(
    Guid BatchId,
    int AppliedCount,
    int ConflictCount,
    List<ConflictDto> Conflicts);
