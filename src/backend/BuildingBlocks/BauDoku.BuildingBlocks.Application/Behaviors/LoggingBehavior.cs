using System.Diagnostics;
using BauDoku.BuildingBlocks.Application.Commands;
using Microsoft.Extensions.Logging;

namespace BauDoku.BuildingBlocks.Application.Behaviors;

public sealed class LoggingBehavior<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _inner;
    private readonly ILogger<LoggingBehavior<TCommand, TResult>> _logger;

    public LoggingBehavior(ICommandHandler<TCommand, TResult> inner, ILogger<LoggingBehavior<TCommand, TResult>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        _logger.LogInformation("Handling command {CommandName}", commandName);

        var stopwatch = Stopwatch.StartNew();
        var result = await _inner.Handle(command, cancellationToken);
        stopwatch.Stop();

        _logger.LogInformation("Command {CommandName} handled in {ElapsedMs}ms", commandName, stopwatch.ElapsedMilliseconds);
        return result;
    }
}

public sealed class LoggingBehaviorVoid<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _inner;
    private readonly ILogger<LoggingBehaviorVoid<TCommand>> _logger;

    public LoggingBehaviorVoid(ICommandHandler<TCommand> inner, ILogger<LoggingBehaviorVoid<TCommand>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        _logger.LogInformation("Handling command {CommandName}", commandName);

        var stopwatch = Stopwatch.StartNew();
        await _inner.Handle(command, cancellationToken);
        stopwatch.Stop();

        _logger.LogInformation("Command {CommandName} handled in {ElapsedMs}ms", commandName, stopwatch.ElapsedMilliseconds);
    }
}
