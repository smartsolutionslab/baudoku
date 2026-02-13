using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ClientInfoTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var client = ClientInfo.Create("Max Mustermann", "max@example.com", "+49 30 12345");

        client.Name.Should().Be("Max Mustermann");
        client.Email.Should().Be("max@example.com");
        client.Phone.Should().Be("+49 30 12345");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string? name)
    {
        var act = () => ClientInfo.Create(name!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', ClientInfo.MaxNameLength + 1);

        var act = () => ClientInfo.Create(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithOptionalFieldsNull_ShouldSucceed()
    {
        var client = ClientInfo.Create("Firma GmbH");

        client.Name.Should().Be("Firma GmbH");
        client.Email.Should().BeNull();
        client.Phone.Should().BeNull();
    }

    [Fact]
    public void Create_WithTooLongEmail_ShouldThrow()
    {
        var longEmail = new string('a', ClientInfo.MaxEmailLength + 1);

        var act = () => ClientInfo.Create("Max", longEmail);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongPhone_ShouldThrow()
    {
        var longPhone = new string('1', ClientInfo.MaxPhoneLength + 1);

        var act = () => ClientInfo.Create("Max", phone: longPhone);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', ClientInfo.MaxNameLength);

        var client = ClientInfo.Create(maxName);

        client.Name.Should().HaveLength(ClientInfo.MaxNameLength);
    }
}
