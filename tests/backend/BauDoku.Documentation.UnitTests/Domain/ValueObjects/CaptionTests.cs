using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class CaptionTests
{
    [Fact]
    public void Create_WithValidCaption_ShouldSucceed()
    {
        var caption = Caption.From("Kabeltrasse vor Verlegung");
        caption.Value.Should().Be("Kabeltrasse vor Verlegung");
    }

    [Fact]
    public void Create_WithMaxLengthCaption_ShouldSucceed()
    {
        var value = new string('a', Caption.MaxLength);
        var caption = Caption.From(value);
        caption.Value.Should().HaveLength(Caption.MaxLength);
    }

    [Fact]
    public void Create_WithTooLongCaption_ShouldThrow()
    {
        var value = new string('a', Caption.MaxLength + 1);
        var act = () => Caption.From(value);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCaption_ShouldThrow(string value)
    {
        var act = () => Caption.From(value);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullCaption_ShouldThrow()
    {
        var act = () => Caption.From(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
