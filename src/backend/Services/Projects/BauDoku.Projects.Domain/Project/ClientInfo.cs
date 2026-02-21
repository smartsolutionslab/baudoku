using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain;

public sealed record ClientInfo : IValueObject
{
    public ClientName Name { get; }
    public EmailAddress? Email { get; }
    public PhoneNumber? Phone { get; }

    private ClientInfo(ClientName name, EmailAddress? email, PhoneNumber? phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }

    public static ClientInfo Create(ClientName name, EmailAddress? email = null, PhoneNumber? phone = null)
    {
        return new ClientInfo(name, email, phone);
    }
}
