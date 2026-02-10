using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.AddPhoto;

public sealed class AddPhotoCommandValidator : AbstractValidator<AddPhotoCommand>
{
    private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg", "image/png", "image/heic"
    ];

    public AddPhotoCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("ContentType muss image/jpeg, image/png oder image/heic sein.");
        RuleFor(x => x.FileSize).GreaterThan(0).LessThanOrEqualTo(MaxFileSize);
        RuleFor(x => x.PhotoType).NotEmpty();
        RuleFor(x => x.Caption).MaximumLength(Domain.ValueObjects.Caption.MaxLength)
            .When(x => x.Caption is not null);
        RuleFor(x => x.Description).MaximumLength(Domain.ValueObjects.Description.MaxLength)
            .When(x => x.Description is not null);
        RuleFor(x => x.Stream).NotNull();
    }
}
