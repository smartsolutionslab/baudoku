using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class GpsQualityGradeTests
{
    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    [InlineData("c")]
    [InlineData("d")]
    public void Create_WithValidValue_ShouldSucceed(string value)
    {
        var grade = GpsQualityGrade.From(value);
        grade.Value.Should().Be(value);
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        GpsQualityGrade.A.Value.Should().Be("a");
        GpsQualityGrade.B.Value.Should().Be("b");
        GpsQualityGrade.C.Value.Should().Be("c");
        GpsQualityGrade.D.Value.Should().Be("d");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => GpsQualityGrade.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("e")]
    [InlineData("A")]
    [InlineData("grade_a")]
    public void Create_WithInvalidValue_ShouldThrow(string value)
    {
        var act = () => GpsQualityGrade.From(value);
        act.Should().Throw<ArgumentException>();
    }
}
