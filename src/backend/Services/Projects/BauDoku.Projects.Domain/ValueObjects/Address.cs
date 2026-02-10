using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record Address : ValueObject
{
    public const int MaxStreetLength = 200;
    public const int MaxCityLength = 100;
    public const int MaxZipCodeLength = 10;

    public string Street { get; }
    public string City { get; }
    public string ZipCode { get; }

    public Address(string street, string city, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Straße darf nicht leer sein.", nameof(street));
        if (street.Length > MaxStreetLength)
            throw new ArgumentException($"Straße darf max. {MaxStreetLength} Zeichen lang sein.", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("Stadt darf nicht leer sein.", nameof(city));
        if (city.Length > MaxCityLength)
            throw new ArgumentException($"Stadt darf max. {MaxCityLength} Zeichen lang sein.", nameof(city));
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("PLZ darf nicht leer sein.", nameof(zipCode));
        if (zipCode.Length > MaxZipCodeLength)
            throw new ArgumentException($"PLZ darf max. {MaxZipCodeLength} Zeichen lang sein.", nameof(zipCode));

        Street = street;
        City = city;
        ZipCode = zipCode;
    }
}
