using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class DescriptionTests
{
    [Fact]
    public void From_WithValidDescription_ShouldSucceed()
    {
        var description = Description.From("Kabeltrasse im Erdgeschoss");

        description.Value.Should().Be("Kabeltrasse im Erdgeschoss");
    }

    [Fact]
    public void From_WithEmptyString_ShouldSucceed()
    {
        var description = Description.From("");

        description.Value.Should().BeEmpty();
    }

    [Fact]
    public void From_WithNull_ShouldThrow()
    {
        var act = () => Description.From(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void From_WithTooLongDescription_ShouldThrow()
    {
        var longDescription = new string('a', Description.MaxLength + 1);

        var act = () => Description.From(longDescription);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLengthDescription_ShouldSucceed()
    {
        var maxDescription = new string('a', Description.MaxLength);

        var description = Description.From(maxDescription);

        description.Value.Should().HaveLength(Description.MaxLength);
    }
}
