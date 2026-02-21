using BauDoku.Sync.Application.Queries.Dtos;

namespace BauDoku.Sync.Application.Queries;

public sealed record ChangeSetResult(
    List<ServerDeltaDto> Changes,
    DateTime ServerTimestamp,
    bool HasMore);
