using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class GpsSourceTests
{
    [Fact]
    public void From_WithValidSource_ShouldSucceed()
    {
        var source = GpsSource.From("internal_gps");
        source.Value.Should().Be("internal_gps");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => GpsSource.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', GpsSource.MaxLength + 1);
        var act = () => GpsSource.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        GpsSource.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        GpsSource.FromNullable("external_dgnss")!.Value.Should().Be("external_dgnss");
    }
}
