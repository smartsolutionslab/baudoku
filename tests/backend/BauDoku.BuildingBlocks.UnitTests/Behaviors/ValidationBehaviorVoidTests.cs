using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Behaviors;
using BauDoku.BuildingBlocks.Application.Commands;
using FluentValidation;
using NSubstitute;

namespace BauDoku.BuildingBlocks.UnitTests.Behaviors;

public sealed class ValidationBehaviorVoidTests
{
    public sealed record TestVoidCommand(string Name) : ICommand;

    private sealed class TestVoidCommandValidator : AbstractValidator<TestVoidCommand>
    {
        public TestVoidCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name darf nicht leer sein.");
        }
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCallInnerHandler()
    {
        var inner = Substitute.For<ICommandHandler<TestVoidCommand>>();
        var validator = new TestVoidCommandValidator();
        var behavior = new ValidationBehaviorVoid<TestVoidCommand>(inner, [validator]);

        await behavior.Handle(new TestVoidCommand("Valid"), CancellationToken.None);

        await inner.Received(1).Handle(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
    {
        var inner = Substitute.For<ICommandHandler<TestVoidCommand>>();
        var validator = new TestVoidCommandValidator();
        var behavior = new ValidationBehaviorVoid<TestVoidCommand>(inner, [validator]);

        Func<Task> act = () => behavior.Handle(new TestVoidCommand(""), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Name"));
        await inner.DidNotReceive().Handle(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNoValidators_ShouldCallInnerHandler()
    {
        var inner = Substitute.For<ICommandHandler<TestVoidCommand>>();
        var behavior = new ValidationBehaviorVoid<TestVoidCommand>(inner, Enumerable.Empty<IValidator<TestVoidCommand>>());

        await behavior.Handle(new TestVoidCommand("Valid"), CancellationToken.None);

        await inner.Received(1).Handle(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>());
    }
}
