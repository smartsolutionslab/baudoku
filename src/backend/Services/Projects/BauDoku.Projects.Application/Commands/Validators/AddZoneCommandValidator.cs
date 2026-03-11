using FluentValidation;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Commands.Validators;

public sealed class AddZoneCommandValidator : AbstractValidator<AddZoneCommand>
{
    public AddZoneCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotNull();
        RuleFor(x => x.Name).NotNull();
        RuleFor(x => x.Type).NotNull();
    }
}
