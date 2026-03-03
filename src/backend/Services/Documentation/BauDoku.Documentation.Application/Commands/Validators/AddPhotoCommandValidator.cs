using SmartSolutionsLab.BauDoku.Documentation.Application.Constants;
using FluentValidation;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Validators;

public sealed class AddPhotoCommandValidator : AbstractValidator<AddPhotoCommand>
{
    public AddPhotoCommandValidator()
    {
        RuleFor(x => x.ContentType)
            .Must(ct => PhotoUploadConstants.AllowedContentTypes.Contains(ct.Value))
            .WithMessage("ContentType muss image/jpeg, image/png oder image/heic sein.");
        RuleFor(x => x.FileSize)
            .Must(fs => fs.Value <= PhotoUploadConstants.MaxFileSize)
            .WithMessage($"Dateigröße darf max. {PhotoUploadConstants.MaxFileSize} Bytes betragen.");
        RuleFor(x => x.Stream).NotNull();
    }
}
