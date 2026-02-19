using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record Address : IValueObject
{
    public Street Street { get; }
    public City City { get; }
    public ZipCode ZipCode { get; }

    private Address(Street street, City city, ZipCode zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }

    public static Address Create(Street street, City city, ZipCode zipCode)
    {
        return new Address(street, city, zipCode);
    }
}
