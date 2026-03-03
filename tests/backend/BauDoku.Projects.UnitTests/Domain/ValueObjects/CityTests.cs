using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class CityTests
{
    [Fact]
    public void From_WithValidCity_ShouldSucceed()
    {
        var city = City.From("Berlin");
        city.Value.Should().Be("Berlin");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => City.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', City.MaxLength + 1);
        var act = () => City.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLength_ShouldSucceed()
    {
        var maxValue = new string('a', City.MaxLength);
        var city = City.From(maxValue);
        city.Value.Should().HaveLength(City.MaxLength);
    }
}
