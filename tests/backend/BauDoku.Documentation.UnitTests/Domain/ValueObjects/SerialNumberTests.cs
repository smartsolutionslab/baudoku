using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class SerialNumberTests
{
    [Fact]
    public void From_WithValidNumber_ShouldSucceed()
    {
        var serial = SerialNumber.From("SN-12345");

        serial.Value.Should().Be("SN-12345");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyNumber_ShouldThrow(string? value)
    {
        var act = () => SerialNumber.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongNumber_ShouldThrow()
    {
        var longNumber = new string('1', SerialNumber.MaxLength + 1);

        var act = () => SerialNumber.From(longNumber);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLengthNumber_ShouldSucceed()
    {
        var maxNumber = new string('1', SerialNumber.MaxLength);

        var serial = SerialNumber.From(maxNumber);

        serial.Value.Should().HaveLength(SerialNumber.MaxLength);
    }
}
