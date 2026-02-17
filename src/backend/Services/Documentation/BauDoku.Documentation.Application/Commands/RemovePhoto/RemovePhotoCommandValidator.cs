using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.RemovePhoto;

public sealed class RemovePhotoCommandValidator : AbstractValidator<RemovePhotoCommand>
{
    public RemovePhotoCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotEmpty();
        RuleFor(x => x.PhotoId).NotEmpty();
    }
}
