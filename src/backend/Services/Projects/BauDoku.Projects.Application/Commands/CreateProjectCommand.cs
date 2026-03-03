using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Commands;

public sealed record CreateProjectCommand(
    ProjectName Name,
    Street Street,
    City City,
    ZipCode ZipCode,
    ClientName ClientName,
    EmailAddress? ClientEmail,
    PhoneNumber? ClientPhone) : ICommand<ProjectIdentifier>;
