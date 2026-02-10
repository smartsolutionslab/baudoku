using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Projects.Application.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Name,
    string Street,
    string City,
    string ZipCode,
    string ClientName,
    string? ClientEmail,
    string? ClientPhone) : ICommand<Guid>;
