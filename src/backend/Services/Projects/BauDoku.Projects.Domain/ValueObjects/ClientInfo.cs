using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ClientInfo : ValueObject
{
    public const int MaxNameLength = 200;
    public const int MaxEmailLength = 254;
    public const int MaxPhoneLength = 30;

    public string Name { get; }
    public string? Email { get; }
    public string? Phone { get; }

    public ClientInfo(string name, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Kundenname darf nicht leer sein.", nameof(name));
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Kundenname darf max. {MaxNameLength} Zeichen lang sein.", nameof(name));
        if (email is not null && email.Length > MaxEmailLength)
            throw new ArgumentException($"E-Mail darf max. {MaxEmailLength} Zeichen lang sein.", nameof(email));
        if (phone is not null && phone.Length > MaxPhoneLength)
            throw new ArgumentException($"Telefon darf max. {MaxPhoneLength} Zeichen lang sein.", nameof(phone));

        Name = name;
        Email = email;
        Phone = phone;
    }
}
