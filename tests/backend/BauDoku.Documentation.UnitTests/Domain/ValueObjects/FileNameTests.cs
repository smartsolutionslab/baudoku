using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class FileNameTests
{
    [Fact]
    public void From_WithValidName_ShouldSucceed()
    {
        var name = FileName.From("photo_001.jpg");
        name.Value.Should().Be("photo_001.jpg");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => FileName.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', FileName.MaxLength + 1);
        var act = () => FileName.From(longValue);
        act.Should().Throw<ArgumentException>();
    }
}
