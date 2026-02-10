using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.RemovePhoto;

public sealed record RemovePhotoCommand(
    Guid InstallationId,
    Guid PhotoId) : ICommand;
