using BauDoku.Sync.Application.Commands.ResolveConflict;
using BauDoku.Sync.Domain;
using FluentValidation.TestHelper;

namespace BauDoku.Sync.UnitTests.Application.Validators;

public sealed class ResolveConflictCommandValidatorTests
{
    private readonly ResolveConflictCommandValidator validator = new();

    private static ResolveConflictCommand CreateValidCommand() =>
        new(ConflictRecordIdentifier.New(), ConflictResolutionStrategy.ClientWins, null);

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ManualMerge_WithoutPayload_ShouldHaveError()
    {
        var cmd = new ResolveConflictCommand(ConflictRecordIdentifier.New(), ConflictResolutionStrategy.ManualMerge, null);
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.MergedPayload);
    }

    [Fact]
    public void ManualMerge_WithPayload_ShouldNotHaveError()
    {
        var cmd = new ResolveConflictCommand(ConflictRecordIdentifier.New(), ConflictResolutionStrategy.ManualMerge, DeltaPayload.From("""{"merged":"data"}"""));
        validator.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.MergedPayload);
    }
}
