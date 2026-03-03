using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class AltitudeTests
{
    [Fact]
    public void From_WithPositiveValue_ShouldSucceed()
    {
        var altitude = Altitude.From(520.0);
        altitude.Value.Should().Be(520.0);
    }

    [Fact]
    public void From_WithZero_ShouldSucceed()
    {
        var altitude = Altitude.From(0.0);
        altitude.Value.Should().Be(0.0);
    }

    [Fact]
    public void From_WithNegativeValue_ShouldSucceed()
    {
        var altitude = Altitude.From(-50.0);
        altitude.Value.Should().Be(-50.0);
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnAltitude()
    {
        var altitude = Altitude.FromNullable(520.0);
        altitude.Should().NotBeNull();
        altitude!.Value.Should().Be(520.0);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        var altitude = Altitude.FromNullable(null);
        altitude.Should().BeNull();
    }
}
