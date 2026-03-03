using SmartSolutionsLab.BauDoku.Sync.ReadModel;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Queries;

public sealed record ChangeSetResult(
    List<ServerDeltaDto> Changes,
    DateTime ServerTimestamp,
    bool HasMore);
