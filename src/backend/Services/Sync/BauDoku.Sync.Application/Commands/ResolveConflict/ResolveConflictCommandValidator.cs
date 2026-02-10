using BauDoku.Sync.Domain.ValueObjects;
using FluentValidation;

namespace BauDoku.Sync.Application.Commands.ResolveConflict;

public sealed class ResolveConflictCommandValidator : AbstractValidator<ResolveConflictCommand>
{
    public ResolveConflictCommandValidator()
    {
        RuleFor(x => x.ConflictId).NotEmpty();
        RuleFor(x => x.Strategy).NotEmpty();
        RuleFor(x => x.MergedPayload)
            .NotEmpty()
            .MaximumLength(DeltaPayload.MaxLength)
            .When(x => x.Strategy == "manual_merge");
    }
}
