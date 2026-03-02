using BauDoku.BuildingBlocks.Application.Commands;
using FluentValidation;

namespace BauDoku.BuildingBlocks.Application.Behaviors;

public sealed class ValidationBehavior<TCommand, TResult>(ICommandHandler<TCommand, TResult> inner, IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default)
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

        return await inner.Handle(command, cancellationToken);
    }
}
