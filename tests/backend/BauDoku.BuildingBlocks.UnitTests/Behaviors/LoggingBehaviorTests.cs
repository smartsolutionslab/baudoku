using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Behaviors;
using BauDoku.BuildingBlocks.Application.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.BuildingBlocks.UnitTests.Behaviors;

public sealed class LoggingBehaviorTests
{
    public sealed record TestCommand(string Name) : ICommand<string>;

    public sealed record TestVoidCommand(string Name) : ICommand;

    [Fact]
    public async Task Handle_ShouldCallInnerAndReturnResult()
    {
        var inner = Substitute.For<ICommandHandler<TestCommand, string>>();
        inner.Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns("result");
        var logger = Substitute.For<ILogger<LoggingBehavior<TestCommand, string>>>();

        var behavior = new LoggingBehavior<TestCommand, string>(inner, logger);

        var result = await behavior.Handle(new TestCommand("test"));

        result.Should().Be("result");
        await inner.Received(1).Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInnerThrows_ShouldPropagateException()
    {
        var inner = Substitute.For<ICommandHandler<TestCommand, string>>();
        inner.Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("test error"));
        var logger = Substitute.For<ILogger<LoggingBehavior<TestCommand, string>>>();

        var behavior = new LoggingBehavior<TestCommand, string>(inner, logger);

        var act = () => behavior.Handle(new TestCommand("test"));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("test error");
    }

    [Fact]
    public async Task HandleVoid_ShouldCallInner()
    {
        var inner = Substitute.For<ICommandHandler<TestVoidCommand>>();
        var logger = Substitute.For<ILogger<LoggingBehaviorVoid<TestVoidCommand>>>();

        var behavior = new LoggingBehaviorVoid<TestVoidCommand>(inner, logger);

        await behavior.Handle(new TestVoidCommand("test"));

        await inner.Received(1).Handle(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleVoid_WhenInnerThrows_ShouldPropagateException()
    {
        var inner = Substitute.For<ICommandHandler<TestVoidCommand>>();
        inner.Handle(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("test error"));
        var logger = Substitute.For<ILogger<LoggingBehaviorVoid<TestVoidCommand>>>();

        var behavior = new LoggingBehaviorVoid<TestVoidCommand>(inner, logger);

        var act = () => behavior.Handle(new TestVoidCommand("test"));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("test error");
    }
}
