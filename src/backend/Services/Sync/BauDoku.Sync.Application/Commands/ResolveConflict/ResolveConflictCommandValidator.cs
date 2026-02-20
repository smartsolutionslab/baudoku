using BauDoku.Sync.Domain;
using FluentValidation;

namespace BauDoku.Sync.Application.Commands.ResolveConflict;

public sealed class ResolveConflictCommandValidator : AbstractValidator<ResolveConflictCommand>
{
    public ResolveConflictCommandValidator()
    {
        RuleFor(x => x.MergedPayload)
            .NotNull()
            .When(x => x.Strategy == ConflictResolutionStrategy.ManualMerge)
            .WithMessage("Merged-Payload wird bei ManualMerge benötigt.");
    }
}
