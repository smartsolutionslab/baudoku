using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class GpsPositionTests
{
    [Fact]
    public void Create_WithValidPosition_ShouldSucceed()
    {
        var position = GpsPosition.Create(48.1351, 11.5820, 520.0, 3.5, "internal_gps");
        position.Latitude.Should().Be(48.1351);
        position.Longitude.Should().Be(11.5820);
        position.Altitude.Should().Be(520.0);
        position.HorizontalAccuracy.Should().Be(3.5);
        position.Source.Value.Should().Be("internal_gps");
    }

    [Fact]
    public void Create_WithAllOptionalFields_ShouldSucceed()
    {
        var position = GpsPosition.Create(
            48.1351, 11.5820, 520.0, 0.03, "rtk",
            "sapos_heps", "fix", 14, 0.8, 1.2);

        position.CorrectionService!.Value.Should().Be("sapos_heps");
        position.RtkFixStatus!.Value.Should().Be("fix");
        position.SatelliteCount.Should().Be(14);
        position.Hdop.Should().Be(0.8);
        position.CorrectionAge.Should().Be(1.2);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Create_WithInvalidLatitude_ShouldThrow(double latitude)
    {
        var act = () => GpsPosition.Create(latitude, 11.0, null, 3.5, "internal_gps");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Create_WithInvalidLongitude_ShouldThrow(double longitude)
    {
        var act = () => GpsPosition.Create(48.0, longitude, null, 3.5, "internal_gps");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidAccuracy_ShouldThrow(double accuracy)
    {
        var act = () => GpsPosition.Create(48.0, 11.0, null, accuracy, "internal_gps");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptySource_ShouldThrow(string? source)
    {
        var act = () => GpsPosition.Create(48.0, 11.0, null, 3.5, source!);
        act.Should().Throw<ArgumentException>();
    }
}
