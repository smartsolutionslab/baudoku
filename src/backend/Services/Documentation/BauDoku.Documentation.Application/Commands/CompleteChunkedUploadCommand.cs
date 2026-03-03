using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands;

public sealed record CompleteChunkedUploadCommand(UploadSessionIdentifier SessionId) : ICommand<PhotoIdentifier>;
