using BauDoku.Documentation.Application.Commands.CompleteInstallation;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class CompleteInstallationCommandValidatorTests
{
    private readonly CompleteInstallationCommandValidator validator = new();

    [Fact]
    public void InstallationId_WhenEmpty_ShouldHaveError()
    {
        var command = new CompleteInstallationCommand(Guid.Empty);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.InstallationId);
    }

    [Fact]
    public void InstallationId_WhenValid_ShouldNotHaveError()
    {
        var command = new CompleteInstallationCommand(Guid.NewGuid());
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.InstallationId);
    }
}
