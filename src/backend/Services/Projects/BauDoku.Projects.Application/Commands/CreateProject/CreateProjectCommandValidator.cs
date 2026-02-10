using BauDoku.Projects.Domain.ValueObjects;
using FluentValidation;

namespace BauDoku.Projects.Application.Commands.CreateProject;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(ProjectName.MaxLength);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(Address.MaxStreetLength);
        RuleFor(x => x.City).NotEmpty().MaximumLength(Address.MaxCityLength);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(Address.MaxZipCodeLength);
        RuleFor(x => x.ClientName).NotEmpty().MaximumLength(ClientInfo.MaxNameLength);
        RuleFor(x => x.ClientEmail).MaximumLength(ClientInfo.MaxEmailLength).When(x => x.ClientEmail is not null);
        RuleFor(x => x.ClientPhone).MaximumLength(ClientInfo.MaxPhoneLength).When(x => x.ClientPhone is not null);
    }
}
