using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using FluentValidation;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Behaviors;

public sealed class ValidationBehaviorVoid<TCommand>(ICommandHandler<TCommand> inner, IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        var validatorList = validators as IReadOnlyList<IValidator<TCommand>> ?? validators.ToList();
        if (validatorList.Count > 0)
        {
            var context = new ValidationContext<TCommand>(command);
            var validationResults = await Task.WhenAll(validatorList.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count != 0) throw new ValidationException(failures);
        }

        await inner.Handle(command, cancellationToken);
    }
}
