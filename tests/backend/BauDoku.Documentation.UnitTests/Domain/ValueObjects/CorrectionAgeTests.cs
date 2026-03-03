using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class CorrectionAgeTests
{
    [Fact]
    public void From_WithPositiveValue_ShouldSucceed()
    {
        var age = CorrectionAge.From(1.2);
        age.Value.Should().Be(1.2);
    }

    [Fact]
    public void From_WithZero_ShouldSucceed()
    {
        var age = CorrectionAge.From(0.0);
        age.Value.Should().Be(0.0);
    }

    [Fact]
    public void From_WithNegativeValue_ShouldThrow()
    {
        var act = () => CorrectionAge.From(-0.5);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnCorrectionAge()
    {
        var age = CorrectionAge.FromNullable(2.0);
        age.Should().NotBeNull();
        age!.Value.Should().Be(2.0);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        var age = CorrectionAge.FromNullable(null);
        age.Should().BeNull();
    }
}
