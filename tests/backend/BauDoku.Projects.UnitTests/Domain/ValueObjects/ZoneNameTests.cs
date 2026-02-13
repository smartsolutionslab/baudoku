using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ZoneNameTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var name = new ZoneName("Erdgeschoss");

        name.Value.Should().Be("Erdgeschoss");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string? value)
    {
        var act = () => new ZoneName(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', ZoneName.MaxLength + 1);

        var act = () => new ZoneName(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', ZoneName.MaxLength);

        var name = new ZoneName(maxName);

        name.Value.Should().HaveLength(ZoneName.MaxLength);
    }
}
