using System.Diagnostics;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using Microsoft.Extensions.Logging;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Behaviors;

public sealed class LoggingBehaviorVoid<TCommand>(ICommandHandler<TCommand> inner, ILogger<LoggingBehaviorVoid<TCommand>> logger)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        BehaviorLogMessages.LogHandlingCommand(logger, commandName);

        var stopwatch = Stopwatch.StartNew();
        await inner.Handle(command, cancellationToken);
        stopwatch.Stop();

        BehaviorLogMessages.LogCommandHandled(logger, commandName, stopwatch.ElapsedMilliseconds);
    }
}
