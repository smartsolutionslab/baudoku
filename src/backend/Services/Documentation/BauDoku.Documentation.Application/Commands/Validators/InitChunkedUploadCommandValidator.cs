using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.Validators;

public sealed class InitChunkedUploadCommandValidator : AbstractValidator<InitChunkedUploadCommand>
{
    private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB
    private const int MaxChunks = 50;

    private static readonly HashSet<string> AllowedContentTypes = ["image/jpeg", "image/png", "image/heic"];

    public InitChunkedUploadCommandValidator()
    {
        RuleFor(x => x.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct.Value))
            .WithMessage("ContentType muss image/jpeg, image/png oder image/heic sein.");
        RuleFor(x => x.TotalSize)
            .Must(fs => fs.Value <= MaxFileSize)
            .WithMessage($"Dateigröße darf max. {MaxFileSize} Bytes betragen.");
        RuleFor(x => x.TotalChunks).GreaterThan(0).LessThanOrEqualTo(MaxChunks);
    }
}
