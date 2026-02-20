using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Commands;

public sealed record CreateProjectCommand(
    ProjectName Name,
    Street Street,
    City City,
    ZipCode ZipCode,
    ClientName ClientName,
    EmailAddress? ClientEmail,
    PhoneNumber? ClientPhone) : ICommand<Guid>;
