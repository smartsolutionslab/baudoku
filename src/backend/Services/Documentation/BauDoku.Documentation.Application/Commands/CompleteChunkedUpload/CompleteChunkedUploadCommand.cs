using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.CompleteChunkedUpload;

public sealed record CompleteChunkedUploadCommand(Guid SessionId) : ICommand<Guid>;
