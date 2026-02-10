using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class DeviceIdTests
{
    [Fact]
    public void Create_WithValidValue_ShouldSucceed()
    {
        var id = new DeviceId("device-123");
        id.Value.Should().Be("device-123");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => new DeviceId(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', DeviceId.MaxLength + 1);
        var act = () => new DeviceId(longValue);
        act.Should().Throw<ArgumentException>();
    }
}
