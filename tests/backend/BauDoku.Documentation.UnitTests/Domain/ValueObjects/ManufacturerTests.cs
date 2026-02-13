using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ManufacturerTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var manufacturer = new Manufacturer("Hager");

        manufacturer.Value.Should().Be("Hager");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string? value)
    {
        var act = () => new Manufacturer(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', Manufacturer.MaxLength + 1);

        var act = () => new Manufacturer(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', Manufacturer.MaxLength);

        var manufacturer = new Manufacturer(maxName);

        manufacturer.Value.Should().HaveLength(Manufacturer.MaxLength);
    }
}
