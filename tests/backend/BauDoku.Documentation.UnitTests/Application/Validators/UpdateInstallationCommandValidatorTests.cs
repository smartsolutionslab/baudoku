using BauDoku.Documentation.Application.Commands.UpdateInstallation;
using BauDoku.Documentation.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class UpdateInstallationCommandValidatorTests
{
    private readonly UpdateInstallationCommandValidator validator = new();

    private static UpdateInstallationCommand CreateValidCommand() =>
        new(Guid.NewGuid(),
            Latitude: 48.1351, Longitude: 11.5820, Altitude: 520.0,
            HorizontalAccuracy: 3.5, GpsSource: "internal_gps",
            CorrectionService: null, RtkFixStatus: null,
            SatelliteCount: null, Hdop: null, CorrectionAge: null,
            Description: "Test", CableType: "NYM-J",
            CrossSection: 2.5m, CableColor: null, ConductorCount: null,
            DepthMm: 600);

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

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Latitude_WhenOutOfRange_ShouldHaveError(double latitude)
    {
        var cmd = CreateValidCommand() with { Latitude = latitude };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Longitude_WhenOutOfRange_ShouldHaveError(double longitude)
    {
        var cmd = CreateValidCommand() with { Longitude = longitude };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Longitude);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void HorizontalAccuracy_WhenZeroOrNegative_ShouldHaveError(double accuracy)
    {
        var cmd = CreateValidCommand() with { HorizontalAccuracy = accuracy };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.HorizontalAccuracy);
    }

    [Fact]
    public void Description_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Description = new string('a', Description.MaxLength + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void CableType_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { CableType = new string('a', CableSpec.MaxCableTypeLength + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.CableType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CrossSection_WhenZeroOrNegative_ShouldHaveError(decimal crossSection)
    {
        var cmd = CreateValidCommand() with { CrossSection = crossSection };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.CrossSection);
    }

    [Fact]
    public void DepthMm_WhenNegative_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { DepthMm = -1 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.DepthMm);
    }
}
