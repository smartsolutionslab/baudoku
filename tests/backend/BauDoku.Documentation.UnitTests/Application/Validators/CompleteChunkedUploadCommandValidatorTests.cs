using BauDoku.Documentation.Application.Commands.CompleteChunkedUpload;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class CompleteChunkedUploadCommandValidatorTests
{
    private readonly CompleteChunkedUploadCommandValidator validator = new();

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(new CompleteChunkedUploadCommand(Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SessionId_WhenEmpty_ShouldHaveError()
    {
        var result = validator.TestValidate(new CompleteChunkedUploadCommand(Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.SessionId);
    }
}
