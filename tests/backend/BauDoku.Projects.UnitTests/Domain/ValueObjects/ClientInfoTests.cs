using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ClientInfoTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var client = ClientInfo.Create(
            ClientName.From("Max Mustermann"),
            EmailAddress.From("max@example.com"),
            PhoneNumber.From("+49 30 12345"));

        client.Name.Value.Should().Be("Max Mustermann");
        client.Email!.Value.Should().Be("max@example.com");
        client.Phone!.Value.Should().Be("+49 30 12345");
    }

    [Fact]
    public void Create_WithOptionalFieldsNull_ShouldSucceed()
    {
        var client = ClientInfo.Create(ClientName.From("Firma GmbH"));

        client.Name.Value.Should().Be("Firma GmbH");
        client.Email.Should().BeNull();
        client.Phone.Should().BeNull();
    }
}
