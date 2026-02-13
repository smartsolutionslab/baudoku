using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class SerialNumberTests
{
    [Fact]
    public void Create_WithValidNumber_ShouldSucceed()
    {
        var serial = new SerialNumber("SN-12345");

        serial.Value.Should().Be("SN-12345");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyNumber_ShouldThrow(string? value)
    {
        var act = () => new SerialNumber(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongNumber_ShouldThrow()
    {
        var longNumber = new string('1', SerialNumber.MaxLength + 1);

        var act = () => new SerialNumber(longNumber);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthNumber_ShouldSucceed()
    {
        var maxNumber = new string('1', SerialNumber.MaxLength);

        var serial = new SerialNumber(maxNumber);

        serial.Value.Should().HaveLength(SerialNumber.MaxLength);
    }
}
