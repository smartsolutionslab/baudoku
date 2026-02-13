using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record Address : ValueObject
{
    public const int MaxStreetLength = 200;
    public const int MaxCityLength = 100;
    public const int MaxZipCodeLength = 10;

    public string Street { get; }
    public string City { get; }
    public string ZipCode { get; }

    private Address(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }

    public static Address Create(string street, string city, string zipCode)
    {
        Ensure.That(street)
            .IsNotNullOrWhiteSpace("Strasse darf nicht leer sein.")
            .MaxLengthIs(MaxStreetLength, $"Strasse darf max. {MaxStreetLength} Zeichen lang sein.");
        Ensure.That(city)
            .IsNotNullOrWhiteSpace("Stadt darf nicht leer sein.")
            .MaxLengthIs(MaxCityLength, $"Stadt darf max. {MaxCityLength} Zeichen lang sein.");
        Ensure.That(zipCode)
            .IsNotNullOrWhiteSpace("PLZ darf nicht leer sein.")
            .MaxLengthIs(MaxZipCodeLength, $"PLZ darf max. {MaxZipCodeLength} Zeichen lang sein.");

        return new Address(street, city, zipCode);
    }
}
