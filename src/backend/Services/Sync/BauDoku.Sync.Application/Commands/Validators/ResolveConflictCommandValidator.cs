using SmartSolutionsLab.BauDoku.Sync.Domain;
using FluentValidation;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Commands.Validators;

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
