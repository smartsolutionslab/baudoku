using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands;

public sealed record CompleteChunkedUploadCommand(UploadSessionIdentifier SessionId) : ICommand<PhotoIdentifier>;
