using AwesomeAssertions;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class EmailAddressTests
{
    [Fact]
    public void From_WithValidEmail_ShouldSucceed()
    {
        var email = EmailAddress.From("max@example.com");

        email.Value.Should().Be("max@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyEmail_ShouldThrow(string? value)
    {
        var act = () => EmailAddress.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongEmail_ShouldThrow()
    {
        var longEmail = new string('a', EmailAddress.MaxLength + 1);

        var act = () => EmailAddress.From(longEmail);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLengthEmail_ShouldSucceed()
    {
        var maxEmail = new string('a', EmailAddress.MaxLength);

        var email = EmailAddress.From(maxEmail);

        email.Value.Should().HaveLength(EmailAddress.MaxLength);
    }
}
