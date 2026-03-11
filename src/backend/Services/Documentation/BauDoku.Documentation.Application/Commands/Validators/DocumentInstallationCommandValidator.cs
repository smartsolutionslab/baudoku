using FluentValidation;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Validators;

public sealed class DocumentInstallationCommandValidator : AbstractValidator<DocumentInstallationCommand>
{
    public DocumentInstallationCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotNull();
        RuleFor(x => x.Type).NotNull();
    }
}
