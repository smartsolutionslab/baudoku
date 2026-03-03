using BauDoku.Sync.Application.ReadModel;

namespace BauDoku.Sync.Application.Queries;

public sealed record ChangeSetResult(
    List<ServerDeltaDto> Changes,
    DateTime ServerTimestamp,
    bool HasMore);
