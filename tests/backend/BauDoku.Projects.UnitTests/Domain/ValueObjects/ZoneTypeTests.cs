using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ZoneTypeTests
{
    [Theory]
    [InlineData("building")]
    [InlineData("floor")]
    [InlineData("room")]
    [InlineData("trench")]
    public void Create_WithValidType_ShouldSucceed(string value)
    {
        var type = new ZoneType(value);

        type.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyType_ShouldThrow(string? value)
    {
        var act = () => new ZoneType(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithInvalidType_ShouldThrow()
    {
        var act = () => new ZoneType("invalid");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        ZoneType.Building.Value.Should().Be("building");
        ZoneType.Floor.Value.Should().Be("floor");
        ZoneType.Room.Value.Should().Be("room");
        ZoneType.Trench.Value.Should().Be("trench");
    }
}
