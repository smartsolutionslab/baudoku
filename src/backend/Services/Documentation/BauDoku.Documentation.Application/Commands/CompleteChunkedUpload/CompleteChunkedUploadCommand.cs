using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.CompleteChunkedUpload;

public sealed record CompleteChunkedUploadCommand(UploadSessionIdentifier SessionId) : ICommand<Guid>;
