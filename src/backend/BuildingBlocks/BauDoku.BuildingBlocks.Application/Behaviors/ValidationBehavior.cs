using BauDoku.BuildingBlocks.Application.Commands;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.BuildingBlocks.Application.Behaviors;

public sealed class ValidationBehavior<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> inner;
    private readonly IEnumerable<IValidator<TCommand>> validators;

    public ValidationBehavior(ICommandHandler<TCommand, TResult> inner, IEnumerable<IValidator<TCommand>> validators)
    {
        this.inner = inner;
        this.validators = validators;
    }

    public async Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TCommand>(command);
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        return await inner.Handle(command, cancellationToken);
    }
}

public sealed class ValidationBehaviorVoid<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> inner;
    private readonly IEnumerable<IValidator<TCommand>> validators;

    public ValidationBehaviorVoid(ICommandHandler<TCommand> inner, IEnumerable<IValidator<TCommand>> validators)
    {
        this.inner = inner;
        this.validators = validators;
    }

    public async Task Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TCommand>(command);
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        await inner.Handle(command, cancellationToken);
    }
}
