using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class DeviceIdentifierTests
{
    [Fact]
    public void Create_WithValidValue_ShouldSucceed()
    {
        var id = DeviceIdentifier.From("device-123");
        id.Value.Should().Be("device-123");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => DeviceIdentifier.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', DeviceIdentifier.MaxLength + 1);
        var act = () => DeviceIdentifier.From(longValue);
        act.Should().Throw<ArgumentException>();
    }
}
