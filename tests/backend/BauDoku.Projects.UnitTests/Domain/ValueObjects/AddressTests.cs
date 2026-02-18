using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var address = Address.Create(Street.From("Musterstraße 1"), City.From("Berlin"), ZipCode.From("10115"));

        address.Street.Value.Should().Be("Musterstraße 1");
        address.City.Value.Should().Be("Berlin");
        address.ZipCode.Value.Should().Be("10115");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyStreet_ShouldThrow(string? street)
    {
        var act = () => Address.Create(Street.From(street!), City.From("Berlin"), ZipCode.From("10115"));

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCity_ShouldThrow(string? city)
    {
        var act = () => Address.Create(Street.From("Musterstraße 1"), City.From(city!), ZipCode.From("10115"));

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyZipCode_ShouldThrow(string? zipCode)
    {
        var act = () => Address.Create(Street.From("Musterstraße 1"), City.From("Berlin"), ZipCode.From(zipCode!));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongStreet_ShouldThrow()
    {
        var longStreet = new string('a', Street.MaxLength + 1);

        var act = () => Address.Create(Street.From(longStreet), City.From("Berlin"), ZipCode.From("10115"));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongCity_ShouldThrow()
    {
        var longCity = new string('a', City.MaxLength + 1);

        var act = () => Address.Create(Street.From("Musterstraße 1"), City.From(longCity), ZipCode.From("10115"));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongZipCode_ShouldThrow()
    {
        var longZip = new string('1', ZipCode.MaxLength + 1);

        var act = () => Address.Create(Street.From("Musterstraße 1"), City.From("Berlin"), ZipCode.From(longZip));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthValues_ShouldSucceed()
    {
        var street = new string('a', Street.MaxLength);
        var city = new string('b', City.MaxLength);
        var zip = new string('1', ZipCode.MaxLength);

        var address = Address.Create(Street.From(street), City.From(city), ZipCode.From(zip));

        address.Street.Value.Should().HaveLength(Street.MaxLength);
        address.City.Value.Should().HaveLength(City.MaxLength);
        address.ZipCode.Value.Should().HaveLength(ZipCode.MaxLength);
    }
}
