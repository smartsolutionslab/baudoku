using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.DeleteInstallation;

public sealed class DeleteInstallationCommandValidator : AbstractValidator<DeleteInstallationCommand>
{
    public DeleteInstallationCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotEmpty();
    }
}
