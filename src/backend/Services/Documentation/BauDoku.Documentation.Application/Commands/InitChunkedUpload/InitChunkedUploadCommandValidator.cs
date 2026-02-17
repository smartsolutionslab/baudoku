using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.InitChunkedUpload;

public sealed class InitChunkedUploadCommandValidator : AbstractValidator<InitChunkedUploadCommand>
{
    private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB
    private const int MaxChunks = 50;

    private static readonly HashSet<string> AllowedContentTypes = ["image/jpeg", "image/png", "image/heic"];

    public InitChunkedUploadCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().Must(ct => AllowedContentTypes.Contains(ct)).WithMessage("ContentType muss image/jpeg, image/png oder image/heic sein.");
        RuleFor(x => x.TotalSize).GreaterThan(0).LessThanOrEqualTo(MaxFileSize);
        RuleFor(x => x.TotalChunks).GreaterThan(0).LessThanOrEqualTo(MaxChunks);
        RuleFor(x => x.PhotoType).NotEmpty();
        RuleFor(x => x.Caption).MaximumLength(Domain.ValueObjects.Caption.MaxLength).When(x => x.Caption is not null);
    }
}
