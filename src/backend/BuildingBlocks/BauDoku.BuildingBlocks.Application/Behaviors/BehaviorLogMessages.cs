using Microsoft.Extensions.Logging;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Behaviors;

internal static partial class BehaviorLogMessages
{
    [LoggerMessage(EventId = 9001, Level = LogLevel.Information,
        Message = "Handling command {CommandName}")]
    public static partial void LogHandlingCommand(ILogger logger, string commandName);

    [LoggerMessage(EventId = 9002, Level = LogLevel.Information,
        Message = "Command {CommandName} handled in {ElapsedMs}ms")]
    public static partial void LogCommandHandled(ILogger logger, string commandName, long elapsedMs);
}
