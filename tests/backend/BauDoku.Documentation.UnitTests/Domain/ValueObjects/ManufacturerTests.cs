using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ManufacturerTests
{
    [Fact]
    public void From_WithValidName_ShouldSucceed()
    {
        var manufacturer = Manufacturer.From("Hager");

        manufacturer.Value.Should().Be("Hager");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyName_ShouldThrow(string? value)
    {
        var act = () => Manufacturer.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', Manufacturer.MaxLength + 1);

        var act = () => Manufacturer.From(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', Manufacturer.MaxLength);

        var manufacturer = Manufacturer.From(maxName);

        manufacturer.Value.Should().HaveLength(Manufacturer.MaxLength);
    }
}
