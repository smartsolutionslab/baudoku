using BauDoku.Documentation.Application.Commands.DocumentInstallation;
using BauDoku.Documentation.Domain;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class DocumentInstallationCommandValidatorTests
{
    private readonly DocumentInstallationCommandValidator validator = new();

    private static DocumentInstallationCommand CreateValidCommand() =>
        new(Guid.NewGuid(), null, "cable_tray",
            48.137154, 11.576124, 520.0, 3.5, "gps",
            null, null, null, null, null,
            "Testbeschreibung", null, null, null, null, null, null, null, null);

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ProjectId_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ProjectId = Guid.Empty };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Type_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Type = "" };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Latitude_WhenBelowMinus90_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Latitude = -91 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Fact]
    public void Latitude_WhenAbove90_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Latitude = 91 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Fact]
    public void Longitude_WhenBelowMinus180_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Longitude = -181 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Longitude);
    }

    [Fact]
    public void Longitude_WhenAbove180_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Longitude = 181 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Longitude);
    }

    [Fact]
    public void HorizontalAccuracy_WhenZero_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { HorizontalAccuracy = 0 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.HorizontalAccuracy);
    }

    [Fact]
    public void HorizontalAccuracy_WhenNegative_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { HorizontalAccuracy = -1 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.HorizontalAccuracy);
    }

    [Fact]
    public void GpsSource_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { GpsSource = "" };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.GpsSource);
    }

    [Fact]
    public void Description_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Description = new string('a', Description.MaxLength + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Manufacturer_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Manufacturer = new string('a', Manufacturer.MaxLength + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Manufacturer);
    }

    [Fact]
    public void CrossSection_WhenZero_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { CrossSection = 0 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.CrossSection);
    }

    [Fact]
    public void DepthMm_WhenNegative_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { DepthMm = -1 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.DepthMm);
    }
}
