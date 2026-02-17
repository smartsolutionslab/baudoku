using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Behaviors;
using BauDoku.BuildingBlocks.Application.Commands;
using FluentValidation;
using NSubstitute;

namespace BauDoku.BuildingBlocks.UnitTests.Behaviors;

public sealed class ValidationBehaviorTests
{
    public sealed record TestCommand(string Name) : ICommand<Guid>;

    private sealed class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name darf nicht leer sein.");
        }
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCallInnerHandler()
    {
        var expectedResult = Guid.NewGuid();
        var inner = Substitute.For<ICommandHandler<TestCommand, Guid>>();
        inner.Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        var validator = new TestCommandValidator();
        var behavior = new ValidationBehavior<TestCommand, Guid>(inner, [validator]);

        var result = await behavior.Handle(new TestCommand("Valid"), CancellationToken.None);

        result.Should().Be(expectedResult);
        await inner.Received(1).Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
    {
        var inner = Substitute.For<ICommandHandler<TestCommand, Guid>>();
        var validator = new TestCommandValidator();
        var behavior = new ValidationBehavior<TestCommand, Guid>(inner, [validator]);

        Func<Task> act = () => behavior.Handle(new TestCommand(""), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Name"));
        await inner.DidNotReceive().Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNoValidators_ShouldCallInnerHandler()
    {
        var expectedResult = Guid.NewGuid();
        var inner = Substitute.For<ICommandHandler<TestCommand, Guid>>();
        inner.Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        var behavior = new ValidationBehavior<TestCommand, Guid>(inner, Enumerable.Empty<IValidator<TestCommand>>());

        var result = await behavior.Handle(new TestCommand("Valid"), CancellationToken.None);

        result.Should().Be(expectedResult);
        await inner.Received(1).Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }
}
