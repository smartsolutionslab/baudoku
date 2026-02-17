using System.Diagnostics;
using BauDoku.BuildingBlocks.Application.Commands;
using Microsoft.Extensions.Logging;

namespace BauDoku.BuildingBlocks.Application.Behaviors;

public sealed class LoggingBehavior<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> inner,
    ILogger<LoggingBehavior<TCommand, TResult>> logger)
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        logger.LogInformation("Handling command {CommandName}", commandName);

        var stopwatch = Stopwatch.StartNew();
        var result = await inner.Handle(command, cancellationToken);
        stopwatch.Stop();

        logger.LogInformation("Command {CommandName} handled in {ElapsedMs}ms", commandName, stopwatch.ElapsedMilliseconds);
        return result;
    }
}
