using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class HdopTests
{
    [Fact]
    public void From_WithPositiveValue_ShouldSucceed()
    {
        var hdop = Hdop.From(0.8);
        hdop.Value.Should().Be(0.8);
    }

    [Fact]
    public void From_WithZero_ShouldThrow()
    {
        var act = () => Hdop.From(0.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void From_WithNegativeValue_ShouldThrow()
    {
        var act = () => Hdop.From(-1.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnHdop()
    {
        var hdop = Hdop.FromNullable(1.5);
        hdop.Should().NotBeNull();
        hdop!.Value.Should().Be(1.5);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        var hdop = Hdop.FromNullable(null);
        hdop.Should().BeNull();
    }
}
