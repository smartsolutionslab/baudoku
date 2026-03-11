using FluentValidation;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Commands.Validators;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotNull();
        RuleFor(x => x.Street).NotNull();
        RuleFor(x => x.City).NotNull();
        RuleFor(x => x.ZipCode).NotNull();
        RuleFor(x => x.ClientName).NotNull();
    }
}
