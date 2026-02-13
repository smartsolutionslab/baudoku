using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ZoneNameTests
{
    [Fact]
    public void From_WithValidName_ShouldSucceed()
    {
        var name = ZoneName.From("Erdgeschoss");

        name.Value.Should().Be("Erdgeschoss");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyName_ShouldThrow(string? value)
    {
        var act = () => ZoneName.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', ZoneName.MaxLength + 1);

        var act = () => ZoneName.From(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', ZoneName.MaxLength);

        var name = ZoneName.From(maxName);

        name.Value.Should().HaveLength(ZoneName.MaxLength);
    }
}
