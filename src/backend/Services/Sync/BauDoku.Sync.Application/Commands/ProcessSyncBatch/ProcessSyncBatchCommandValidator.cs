using BauDoku.Sync.Domain;
using FluentValidation;

namespace BauDoku.Sync.Application.Commands.ProcessSyncBatch;

public sealed class ProcessSyncBatchCommandValidator : AbstractValidator<ProcessSyncBatchCommand>
{
    public ProcessSyncBatchCommandValidator()
    {
        RuleFor(x => x.Deltas).NotEmpty();

        RuleForEach(x => x.Deltas).ChildRules(delta =>
        {
            delta.RuleFor(d => d.EntityType).NotEmpty();
            delta.RuleFor(d => d.EntityId).NotEmpty();
            delta.RuleFor(d => d.Operation).NotEmpty();
            delta.RuleFor(d => d.BaseVersion).GreaterThanOrEqualTo(0);
            delta.RuleFor(d => d.Payload).NotEmpty().MaximumLength(DeltaPayload.MaxLength);
        });
    }
}
