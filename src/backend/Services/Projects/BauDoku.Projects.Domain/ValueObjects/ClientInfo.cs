using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ClientInfo : IValueObject
{
    public const int MaxNameLength = 200;
    public const int MaxEmailLength = 254;
    public const int MaxPhoneLength = 30;

    public string Name { get; }
    public string? Email { get; }
    public string? Phone { get; }

    private ClientInfo(string name, string? email, string? phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }

    public static ClientInfo Create(string name, string? email = null, string? phone = null)
    {
        Ensure.That(name)
            .IsNotNullOrWhiteSpace("Kundenname darf nicht leer sein.")
            .MaxLengthIs(MaxNameLength, $"Kundenname darf max. {MaxNameLength} Zeichen lang sein.");
        Ensure.That(email).MaxLengthIs(MaxEmailLength, $"E-Mail darf max. {MaxEmailLength} Zeichen lang sein.");
        Ensure.That(phone).MaxLengthIs(MaxPhoneLength, $"Telefon darf max. {MaxPhoneLength} Zeichen lang sein.");

        return new ClientInfo(name, email, phone);
    }
}
