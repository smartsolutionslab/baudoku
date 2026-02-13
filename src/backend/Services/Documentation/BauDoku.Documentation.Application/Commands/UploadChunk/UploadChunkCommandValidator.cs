using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.UploadChunk;

public sealed class UploadChunkCommandValidator : AbstractValidator<UploadChunkCommand>
{
    public UploadChunkCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.ChunkIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data).NotNull();
    }
}
