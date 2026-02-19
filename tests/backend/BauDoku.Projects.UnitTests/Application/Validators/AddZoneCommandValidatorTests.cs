using BauDoku.Projects.Application.Commands.AddZone;
using BauDoku.Projects.Domain;
using FluentValidation.TestHelper;

namespace BauDoku.Projects.UnitTests.Application.Validators;

public sealed class AddZoneCommandValidatorTests
{
    private readonly AddZoneCommandValidator validator = new();

    private static AddZoneCommand CreateValidCommand() =>
        new(Guid.NewGuid(), "Erdgeschoss", "floor", null);

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ProjectId_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ProjectId = Guid.Empty };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_WhenEmpty_ShouldHaveError(string? name)
    {
        var cmd = CreateValidCommand() with { Name = name! };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Name = new string('a', ZoneName.MaxLength + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Type_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Type = "" };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Type);
    }
}
