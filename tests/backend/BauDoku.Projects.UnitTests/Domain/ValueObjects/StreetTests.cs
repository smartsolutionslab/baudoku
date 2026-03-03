using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class StreetTests
{
    [Fact]
    public void From_WithValidStreet_ShouldSucceed()
    {
        var street = Street.From("Musterstraße 1");
        street.Value.Should().Be("Musterstraße 1");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => Street.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', Street.MaxLength + 1);
        var act = () => Street.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLength_ShouldSucceed()
    {
        var maxValue = new string('a', Street.MaxLength);
        var street = Street.From(maxValue);
        street.Value.Should().HaveLength(Street.MaxLength);
    }
}
