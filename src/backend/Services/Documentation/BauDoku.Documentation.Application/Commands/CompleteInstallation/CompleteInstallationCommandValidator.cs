using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.CompleteInstallation;

public sealed class CompleteInstallationCommandValidator : AbstractValidator<CompleteInstallationCommand>
{
    public CompleteInstallationCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotEmpty();
    }
}
