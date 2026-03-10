using System.Diagnostics;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using Microsoft.Extensions.Logging;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Behaviors;

public sealed class LoggingBehavior<TCommand, TResult>(ICommandHandler<TCommand, TResult> inner, ILogger<LoggingBehavior<TCommand, TResult>> logger)
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        BehaviorLogMessages.LogHandlingCommand(logger, commandName);

        var stopwatch = Stopwatch.StartNew();
        var result = await inner.Handle(command, cancellationToken);
        stopwatch.Stop();

        BehaviorLogMessages.LogCommandHandled(logger, commandName, stopwatch.ElapsedMilliseconds);
        return result;
    }
}
