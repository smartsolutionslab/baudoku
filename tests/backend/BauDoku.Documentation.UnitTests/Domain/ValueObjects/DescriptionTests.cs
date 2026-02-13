using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class DescriptionTests
{
    [Fact]
    public void Create_WithValidDescription_ShouldSucceed()
    {
        var description = new Description("Kabeltrasse im Erdgeschoss");

        description.Value.Should().Be("Kabeltrasse im Erdgeschoss");
    }

    [Fact]
    public void Create_WithEmptyString_ShouldSucceed()
    {
        var description = new Description("");

        description.Value.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        var act = () => new Description(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithTooLongDescription_ShouldThrow()
    {
        var longDescription = new string('a', Description.MaxLength + 1);

        var act = () => new Description(longDescription);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthDescription_ShouldSucceed()
    {
        var maxDescription = new string('a', Description.MaxLength);

        var description = new Description(maxDescription);

        description.Value.Should().HaveLength(Description.MaxLength);
    }
}
