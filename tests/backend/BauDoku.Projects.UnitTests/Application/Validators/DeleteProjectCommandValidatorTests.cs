using BauDoku.Projects.Application.Commands.DeleteProject;
using FluentValidation.TestHelper;

namespace BauDoku.Projects.UnitTests.Application.Validators;

public sealed class DeleteProjectCommandValidatorTests
{
    private readonly DeleteProjectCommandValidator validator = new();

    [Fact]
    public void ProjectId_WhenEmpty_ShouldHaveError()
    {
        var command = new DeleteProjectCommand(Guid.Empty);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void ProjectId_WhenValid_ShouldNotHaveError()
    {
        var command = new DeleteProjectCommand(Guid.NewGuid());
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectId);
    }
}
