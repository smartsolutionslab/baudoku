using BauDoku.Documentation.Application.Commands.RemoveMeasurement;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class RemoveMeasurementCommandValidatorTests
{
    private readonly RemoveMeasurementCommandValidator validator = new();

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(new RemoveMeasurementCommand(Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InstallationId_WhenEmpty_ShouldHaveError()
    {
        var result = validator.TestValidate(new RemoveMeasurementCommand(Guid.Empty, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.InstallationId);
    }

    [Fact]
    public void MeasurementId_WhenEmpty_ShouldHaveError()
    {
        var result = validator.TestValidate(new RemoveMeasurementCommand(Guid.NewGuid(), Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.MeasurementId);
    }
}
