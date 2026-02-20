using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Commands;

public sealed record ProcessSyncBatchCommand(
    DeviceIdentifier DeviceId,
    List<SyncDeltaDto> Deltas) : ICommand<ProcessSyncBatchResult>;
