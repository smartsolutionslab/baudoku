using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ZipCodeTests
{
    [Fact]
    public void From_WithValidZipCode_ShouldSucceed()
    {
        var zip = ZipCode.From("10115");
        zip.Value.Should().Be("10115");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => ZipCode.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('1', ZipCode.MaxLength + 1);
        var act = () => ZipCode.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLength_ShouldSucceed()
    {
        var maxValue = new string('1', ZipCode.MaxLength);
        var zip = ZipCode.From(maxValue);
        zip.Value.Should().HaveLength(ZipCode.MaxLength);
    }
}
