using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Behaviors;
using BauDoku.BuildingBlocks.Application.Commands;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;

namespace BauDoku.BuildingBlocks.UnitTests.Behaviors;

public sealed class ValidationBehaviorTests
{
    public sealed record TestCommand(string Name) : ICommand<string>;

    public sealed record TestVoidCommand(string Name) : ICommand;

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCallInnerHandler()
    {
        var inner = Substitute.For<ICommandHandler<TestCommand, string>>();
        inner.Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns("result");

        var validator = Substitute.For<IValidator<TestCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<TestCommand, string>(inner, [validator]);

        var result = await behavior.Handle(new TestCommand("test"));

        result.Should().Be("result");
        await inner.Received(1).Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
    {
        var inner = Substitute.For<ICommandHandler<TestCommand, string>>();

        var validator = Substitute.For<IValidator<TestCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Name", "Name is required")]));

        var behavior = new ValidationBehavior<TestCommand, string>(inner, [validator]);

        var act = () => behavior.Handle(new TestCommand(""));

        await act.Should().ThrowAsync<ValidationException>();
        await inner.DidNotReceive().Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNoValidators_ShouldPassThrough()
    {
        var inner = Substitute.For<ICommandHandler<TestCommand, string>>();
        inner.Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns("result");

        var behavior = new ValidationBehavior<TestCommand, string>(inner, []);

        var result = await behavior.Handle(new TestCommand("test"));

        result.Should().Be("result");
        await inner.Received(1).Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleVoid_WithValidCommand_ShouldCallInnerHandler()
    {
        var inner = Substitute.For<ICommandHandler<TestVoidCommand>>();

        var validator = Substitute.For<IValidator<TestVoidCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestVoidCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehaviorVoid<TestVoidCommand>(inner, [validator]);

        await behavior.Handle(new TestVoidCommand("test"));

        await inner.Received(1).Handle(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleVoid_WithInvalidCommand_ShouldThrowValidationException()
    {
        var inner = Substitute.For<ICommandHandler<TestVoidCommand>>();

        var validator = Substitute.For<IValidator<TestVoidCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestVoidCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Name", "Name is required")]));

        var behavior = new ValidationBehaviorVoid<TestVoidCommand>(inner, [validator]);

        var act = () => behavior.Handle(new TestVoidCommand(""));

        await act.Should().ThrowAsync<ValidationException>();
        await inner.DidNotReceive().Handle(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleVoid_WithNoValidators_ShouldPassThrough()
    {
        var inner = Substitute.For<ICommandHandler<TestVoidCommand>>();

        var behavior = new ValidationBehaviorVoid<TestVoidCommand>(inner, []);

        await behavior.Handle(new TestVoidCommand("test"));

        await inner.Received(1).Handle(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>());
    }
}
