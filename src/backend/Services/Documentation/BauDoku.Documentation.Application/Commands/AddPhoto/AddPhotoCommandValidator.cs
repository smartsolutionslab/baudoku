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
        RuleFor(x => x.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct.Value))
            .WithMessage("ContentType muss image/jpeg, image/png oder image/heic sein.");
        RuleFor(x => x.FileSize)
            .Must(fs => fs.Value <= MaxFileSize)
            .WithMessage($"Dateigröße darf max. {MaxFileSize} Bytes betragen.");
        RuleFor(x => x.Stream).NotNull();
    }
}
