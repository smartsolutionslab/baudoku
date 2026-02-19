using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ClientNameTests
{
    [Fact]
    public void From_WithValidName_ShouldSucceed()
    {
        var name = ClientName.From("Max Mustermann");

        name.Value.Should().Be("Max Mustermann");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyName_ShouldThrow(string? value)
    {
        var act = () => ClientName.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', ClientName.MaxLength + 1);

        var act = () => ClientName.From(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', ClientName.MaxLength);

        var name = ClientName.From(maxName);

        name.Value.Should().HaveLength(ClientName.MaxLength);
    }
}
