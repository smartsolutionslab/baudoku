using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.BuildingBlocks.UnitTests.ValueObjects;

public sealed class PageSizeTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(100)]
    public void From_WithValidValue_ShouldCreatePageSize(int value)
    {
        var size = PageSize.From(value);

        size.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(200)]
    public void From_WithOutOfRangeValue_ShouldThrow(int value)
    {
        Action act = () => PageSize.From(value);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Default_ShouldBeTwenty()
    {
        PageSize.Default.Value.Should().Be(20);
    }

    [Fact]
    public void Max_ShouldBeHundred()
    {
        PageSize.Max.Should().Be(100);
    }
}
