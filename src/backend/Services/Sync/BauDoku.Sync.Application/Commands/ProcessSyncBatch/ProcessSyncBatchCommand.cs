using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Sync.Application.Queries.Dtos;

namespace BauDoku.Sync.Application.Commands.ProcessSyncBatch;

public sealed record ProcessSyncBatchCommand(
    string DeviceId,
    List<SyncDeltaDto> Deltas) : ICommand<ProcessSyncBatchResult>;
