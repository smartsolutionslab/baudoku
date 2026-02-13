using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.CompleteChunkedUpload;

public sealed class CompleteChunkedUploadCommandValidator : AbstractValidator<CompleteChunkedUploadCommand>
{
    public CompleteChunkedUploadCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
