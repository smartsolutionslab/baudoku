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
    public void From_WithValidType_ShouldSucceed(string value)
    {
        var type = ZoneType.From(value);

        type.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyType_ShouldThrow(string? value)
    {
        var act = () => ZoneType.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithInvalidType_ShouldThrow()
    {
        var act = () => ZoneType.From("invalid");

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
