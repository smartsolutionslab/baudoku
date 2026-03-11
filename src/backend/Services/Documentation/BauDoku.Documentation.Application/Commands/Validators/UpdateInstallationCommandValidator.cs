using FluentValidation;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Validators;

public sealed class UpdateInstallationCommandValidator : AbstractValidator<UpdateInstallationCommand>
{
    public UpdateInstallationCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotNull();
    }
}
