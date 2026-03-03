using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.UnitTests.ValueObjects;

public sealed class PageNumberTests
{
    [Fact]
    public void From_WithValidValue_ShouldCreatePageNumber()
    {
        var page = PageNumber.From(1);

        page.Value.Should().Be(1);
    }

    [Fact]
    public void From_WithLargeValue_ShouldSucceed()
    {
        var page = PageNumber.From(1000);

        page.Value.Should().Be(1000);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void From_WithZeroOrNegative_ShouldThrow(int value)
    {
        Action act = () => PageNumber.From(value);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Default_ShouldBeOne()
    {
        PageNumber.Default.Value.Should().Be(1);
    }

    [Fact]
    public void FromNullable_WithValue_ShouldCreatePageNumber()
    {
        var page = PageNumber.FromNullable(5);

        page.Value.Should().Be(5);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnDefault()
    {
        var page = PageNumber.FromNullable(null);

        page.Should().Be(PageNumber.Default);
    }
}
