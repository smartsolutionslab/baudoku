using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var address = Address.Create("Musterstraße 1", "Berlin", "10115");

        address.Street.Should().Be("Musterstraße 1");
        address.City.Should().Be("Berlin");
        address.ZipCode.Should().Be("10115");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyStreet_ShouldThrow(string? street)
    {
        var act = () => Address.Create(street!, "Berlin", "10115");

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCity_ShouldThrow(string? city)
    {
        var act = () => Address.Create("Musterstraße 1", city!, "10115");

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyZipCode_ShouldThrow(string? zipCode)
    {
        var act = () => Address.Create("Musterstraße 1", "Berlin", zipCode!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongStreet_ShouldThrow()
    {
        var longStreet = new string('a', Address.MaxStreetLength + 1);

        var act = () => Address.Create(longStreet, "Berlin", "10115");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongCity_ShouldThrow()
    {
        var longCity = new string('a', Address.MaxCityLength + 1);

        var act = () => Address.Create("Musterstraße 1", longCity, "10115");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongZipCode_ShouldThrow()
    {
        var longZip = new string('1', Address.MaxZipCodeLength + 1);

        var act = () => Address.Create("Musterstraße 1", "Berlin", longZip);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthValues_ShouldSucceed()
    {
        var street = new string('a', Address.MaxStreetLength);
        var city = new string('b', Address.MaxCityLength);
        var zip = new string('1', Address.MaxZipCodeLength);

        var address = Address.Create(street, city, zip);

        address.Street.Should().HaveLength(Address.MaxStreetLength);
        address.City.Should().HaveLength(Address.MaxCityLength);
        address.ZipCode.Should().HaveLength(Address.MaxZipCodeLength);
    }
}
