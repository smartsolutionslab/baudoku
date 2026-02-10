using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class DeltaPayloadTests
{
    [Fact]
    public void Create_WithValidJson_ShouldSucceed()
    {
        var json = """{"name":"Test"}""";
        var payload = new DeltaPayload(json);
        payload.Value.Should().Be(json);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => new DeltaPayload(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', DeltaPayload.MaxLength + 1);
        var act = () => new DeltaPayload(longValue);
        act.Should().Throw<ArgumentException>();
    }
}
