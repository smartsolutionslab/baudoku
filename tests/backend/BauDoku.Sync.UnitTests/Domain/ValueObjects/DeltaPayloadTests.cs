using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class DeltaPayloadTests
{
    [Fact]
    public void Create_WithValidJson_ShouldSucceed()
    {
        var json = """{"name":"Test"}""";
        var payload = DeltaPayload.From(json);
        payload.Value.Should().Be(json);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => DeltaPayload.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', DeltaPayload.MaxLength + 1);
        var act = () => DeltaPayload.From(longValue);
        act.Should().Throw<ArgumentException>();
    }
}
