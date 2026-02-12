using BauDoku.Documentation.Application.Commands.RecordMeasurement;
using BauDoku.Documentation.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class RecordMeasurementCommandValidatorTests
{
    private readonly RecordMeasurementCommandValidator _validator = new();

    private static RecordMeasurementCommand CreateValidCommand() =>
        new(Guid.NewGuid(), "insulation_resistance", 500.0, "MΩ", null, null, null);

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = _validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InstallationId_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { InstallationId = Guid.Empty };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.InstallationId);
    }

    [Fact]
    public void Type_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Type = "" };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Value_WhenZero_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Value = 0 };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void Unit_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Unit = "" };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Unit);
    }

    [Fact]
    public void Unit_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Unit = new string('a', MeasurementValue.MaxUnitLength + 1) };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Unit);
    }
}
