using BauDoku.Documentation.Application.Commands.RemovePhoto;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class RemovePhotoCommandValidatorTests
{
    private readonly RemovePhotoCommandValidator validator = new();

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(new RemovePhotoCommand(Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InstallationId_WhenEmpty_ShouldHaveError()
    {
        var result = validator.TestValidate(new RemovePhotoCommand(Guid.Empty, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.InstallationId);
    }

    [Fact]
    public void PhotoId_WhenEmpty_ShouldHaveError()
    {
        var result = validator.TestValidate(new RemovePhotoCommand(Guid.NewGuid(), Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.PhotoId);
    }
}
