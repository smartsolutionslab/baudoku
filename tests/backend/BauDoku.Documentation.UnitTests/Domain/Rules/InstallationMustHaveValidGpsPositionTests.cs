using AwesomeAssertions;
using BauDoku.Documentation.Domain.Rules;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.Rules;

public sealed class InstallationMustHaveValidGpsPositionTests
{
    [Fact]
    public void IsBroken_WithGoodAccuracy_ShouldReturnFalse()
    {
        var position = new GpsPosition(48.1351, 11.5820, 520.0, 5.0, "internal_gps");
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WithExactlyHundredAccuracy_ShouldReturnFalse()
    {
        var position = new GpsPosition(48.1351, 11.5820, 520.0, 100.0, "internal_gps");
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WithSlightlyOverHundredAccuracy_ShouldReturnTrue()
    {
        var position = new GpsPosition(48.1351, 11.5820, 520.0, 100.01, "internal_gps");
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WithPoorAccuracy_ShouldReturnTrue()
    {
        var position = new GpsPosition(48.1351, 11.5820, 520.0, 150.0, "internal_gps");
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void Message_ShouldContainExpectedText()
    {
        var position = new GpsPosition(48.1351, 11.5820, 520.0, 150.0, "internal_gps");
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.Message.Should().Contain("GPS-Position");
    }
}
