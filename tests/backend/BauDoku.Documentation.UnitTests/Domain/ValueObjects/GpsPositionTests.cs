using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class GpsPositionTests
{
    [Fact]
    public void Create_WithValidPosition_ShouldSucceed()
    {
        var position = GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(3.5), GpsSource.From("internal_gps"));
        position.Latitude.Value.Should().Be(48.1351);
        position.Longitude.Value.Should().Be(11.5820);
        position.Altitude.Should().Be(520.0);
        position.HorizontalAccuracy.Value.Should().Be(3.5);
        position.Source.Value.Should().Be("internal_gps");
    }

    [Fact]
    public void Create_WithAllOptionalFields_ShouldSucceed()
    {
        var position = GpsPosition.Create(
            Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(0.03), GpsSource.From("rtk"),
            CorrectionService.From("sapos_heps"), RtkFixStatus.From("fix"), 14, 0.8, 1.2);

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
        var act = () => GpsPosition.Create(Latitude.From(latitude), Longitude.From(11.0), null, HorizontalAccuracy.From(3.5), GpsSource.From("internal_gps"));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Create_WithInvalidLongitude_ShouldThrow(double longitude)
    {
        var act = () => GpsPosition.Create(Latitude.From(48.0), Longitude.From(longitude), null, HorizontalAccuracy.From(3.5), GpsSource.From("internal_gps"));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidAccuracy_ShouldThrow(double accuracy)
    {
        var act = () => GpsPosition.Create(Latitude.From(48.0), Longitude.From(11.0), null, HorizontalAccuracy.From(accuracy), GpsSource.From("internal_gps"));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptySource_ShouldThrow(string? source)
    {
        var act = () => GpsPosition.Create(Latitude.From(48.0), Longitude.From(11.0), null, HorizontalAccuracy.From(3.5), GpsSource.From(source!));
        act.Should().Throw<ArgumentException>();
    }
}
