using BauDoku.Sync.Application.Commands.ResolveConflict;
using BauDoku.Sync.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace BauDoku.Sync.UnitTests.Application.Validators;

public sealed class ResolveConflictCommandValidatorTests
{
    private readonly ResolveConflictCommandValidator validator = new();

    private static ResolveConflictCommand CreateValidCommand() =>
        new(Guid.NewGuid(), "client_wins", null);

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ConflictId_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ConflictId = Guid.Empty };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ConflictId);
    }

    [Fact]
    public void Strategy_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Strategy = "" };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Strategy);
    }

    [Fact]
    public void ManualMerge_WithoutPayload_ShouldHaveError()
    {
        var cmd = new ResolveConflictCommand(Guid.NewGuid(), "manual_merge", null);
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.MergedPayload);
    }

    [Fact]
    public void ManualMerge_WithPayload_ShouldNotHaveError()
    {
        var cmd = new ResolveConflictCommand(Guid.NewGuid(), "manual_merge", """{"merged":"data"}""");
        validator.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.MergedPayload);
    }

    [Fact]
    public void ManualMerge_WithTooLongPayload_ShouldHaveError()
    {
        var cmd = new ResolveConflictCommand(Guid.NewGuid(), "manual_merge", new string('x', DeltaPayload.MaxLength + 1));
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.MergedPayload);
    }
}
