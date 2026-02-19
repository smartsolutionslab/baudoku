using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class PhoneNumberTests
{
    [Fact]
    public void From_WithValidPhone_ShouldSucceed()
    {
        var phone = PhoneNumber.From("+49 30 12345");

        phone.Value.Should().Be("+49 30 12345");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyPhone_ShouldThrow(string? value)
    {
        var act = () => PhoneNumber.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongPhone_ShouldThrow()
    {
        var longPhone = new string('1', PhoneNumber.MaxLength + 1);

        var act = () => PhoneNumber.From(longPhone);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLengthPhone_ShouldSucceed()
    {
        var maxPhone = new string('1', PhoneNumber.MaxLength);

        var phone = PhoneNumber.From(maxPhone);

        phone.Value.Should().HaveLength(PhoneNumber.MaxLength);
    }
}
