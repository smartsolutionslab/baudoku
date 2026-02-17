using BauDoku.Documentation.Application.Commands.DeleteInstallation;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class DeleteInstallationCommandValidatorTests
{
    private readonly DeleteInstallationCommandValidator validator = new();

    [Fact]
    public void InstallationId_WhenEmpty_ShouldHaveError()
    {
        var command = new DeleteInstallationCommand(Guid.Empty);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.InstallationId);
    }

    [Fact]
    public void InstallationId_WhenValid_ShouldNotHaveError()
    {
        var command = new DeleteInstallationCommand(Guid.NewGuid());
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.InstallationId);
    }
}
