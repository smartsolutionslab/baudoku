using BauDoku.Projects.Application.Commands.CreateProject;
using BauDoku.Projects.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace BauDoku.Projects.UnitTests.Application.Validators;

public sealed class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator = new();

    private static CreateProjectCommand CreateValidCommand() =>
        new("Neues Projekt", "Musterstraße 1", "Berlin", "10115", "Max Mustermann", "max@example.com", "+49 30 12345");

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = _validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_WhenEmpty_ShouldHaveError(string? name)
    {
        var cmd = CreateValidCommand() with { Name = name! };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Name = new string('a', ProjectName.MaxLength + 1) };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Street_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Street = "" };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Street);
    }

    [Fact]
    public void Street_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Street = new string('a', Address.MaxStreetLength + 1) };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Street);
    }

    [Fact]
    public void City_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { City = "" };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void City_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { City = new string('a', Address.MaxCityLength + 1) };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void ZipCode_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ZipCode = "" };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ZipCode);
    }

    [Fact]
    public void ZipCode_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ZipCode = new string('1', Address.MaxZipCodeLength + 1) };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ZipCode);
    }

    [Fact]
    public void ClientName_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ClientName = "" };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ClientName);
    }

    [Fact]
    public void ClientName_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ClientName = new string('a', ClientInfo.MaxNameLength + 1) };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ClientName);
    }

    [Fact]
    public void ClientEmail_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ClientEmail = new string('a', ClientInfo.MaxEmailLength + 1) };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ClientEmail);
    }

    [Fact]
    public void ClientEmail_WhenNull_ShouldNotHaveError()
    {
        var cmd = CreateValidCommand() with { ClientEmail = null };
        _validator.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.ClientEmail);
    }

    [Fact]
    public void ClientPhone_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ClientPhone = new string('1', ClientInfo.MaxPhoneLength + 1) };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ClientPhone);
    }
}
