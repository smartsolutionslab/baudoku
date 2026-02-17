using BauDoku.Documentation.Application.Commands.RecordMeasurement;
using BauDoku.Documentation.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class RecordMeasurementCommandValidatorTests
{
    private readonly RecordMeasurementCommandValidator validator = new();

    private static RecordMeasurementCommand CreateValidCommand() =>
        new(Guid.NewGuid(), "insulation_resistance", 500.0, "MΩ", null, null, null);

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InstallationId_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { InstallationId = Guid.Empty };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.InstallationId);
    }

    [Fact]
    public void Type_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Type = "" };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Value_WhenZero_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Value = 0 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void Unit_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Unit = "" };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Unit);
    }

    [Fact]
    public void Unit_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Unit = new string('a', MeasurementUnit.MaxLength + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Unit);
    }
}
