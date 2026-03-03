using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class CrossSectionTests
{
    [Fact]
    public void From_WithValidValue_ShouldSucceed()
    {
        var cs = CrossSection.From(2.5m);
        cs.Value.Should().Be(2.5m);
    }

    [Fact]
    public void From_WithZero_ShouldThrow()
    {
        var act = () => CrossSection.From(0m);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void From_WithNegative_ShouldThrow()
    {
        var act = () => CrossSection.From(-1.5m);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        CrossSection.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        CrossSection.FromNullable(4.0m)!.Value.Should().Be(4.0m);
    }
}
