using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class CorrectionServiceTests
{
    [Fact]
    public void From_WithValidValue_ShouldSucceed()
    {
        var cs = CorrectionService.From("SAPOS-EPS");
        cs.Value.Should().Be("SAPOS-EPS");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => CorrectionService.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', CorrectionService.MaxLength + 1);
        var act = () => CorrectionService.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        CorrectionService.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        CorrectionService.FromNullable("SAPOS-HEPS")!.Value.Should().Be("SAPOS-HEPS");
    }
}
