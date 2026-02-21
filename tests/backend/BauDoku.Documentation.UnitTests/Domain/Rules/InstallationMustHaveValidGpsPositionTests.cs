using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.Rules;

public sealed class InstallationMustHaveValidGpsPositionTests
{
    [Fact]
    public void IsBroken_WithGoodAccuracy_ShouldReturnFalse()
    {
        var position = GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(5.0), GpsSource.From("internal_gps"));
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WithExactlyHundredAccuracy_ShouldReturnFalse()
    {
        var position = GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(100.0), GpsSource.From("internal_gps"));
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WithSlightlyOverHundredAccuracy_ShouldReturnTrue()
    {
        var position = GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(100.01), GpsSource.From("internal_gps"));
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WithPoorAccuracy_ShouldReturnTrue()
    {
        var position = GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(150.0), GpsSource.From("internal_gps"));
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void Message_ShouldContainExpectedText()
    {
        var position = GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(150.0), GpsSource.From("internal_gps"));
        var rule = new InstallationMustHaveValidGpsPosition(position);

        rule.Message.Should().Contain("GPS-Position");
    }
}
