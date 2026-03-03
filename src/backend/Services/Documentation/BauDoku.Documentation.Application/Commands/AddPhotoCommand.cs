using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands;

public sealed record AddPhotoCommand(
    InstallationIdentifier InstallationId,
    FileName FileName,
    ContentType ContentType,
    FileSize FileSize,
    PhotoType PhotoType,
    Caption? Caption,
    Description? Description,
    GpsPosition? Position,
    Stream Stream,
    DateTime? TakenAt = null) : ICommand<PhotoIdentifier>;
