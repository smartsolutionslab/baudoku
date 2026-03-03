using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Sync.ReadModel;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Commands;

public sealed record ProcessSyncBatchCommand(
    DeviceIdentifier DeviceId,
    List<SyncDeltaDto> Deltas) : ICommand<ProcessSyncBatchResult>;
