using BauDoku.Projects.Domain;
using FluentValidation;

namespace BauDoku.Projects.Application.Commands.CreateProject;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(ProjectName.MaxLength);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(Street.MaxLength);
        RuleFor(x => x.City).NotEmpty().MaximumLength(City.MaxLength);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(ZipCode.MaxLength);
        RuleFor(x => x.ClientName).NotEmpty().MaximumLength(ClientName.MaxLength);
        RuleFor(x => x.ClientEmail).MaximumLength(EmailAddress.MaxLength).When(x => x.ClientEmail is not null);
        RuleFor(x => x.ClientPhone).MaximumLength(PhoneNumber.MaxLength).When(x => x.ClientPhone is not null);
    }
}
