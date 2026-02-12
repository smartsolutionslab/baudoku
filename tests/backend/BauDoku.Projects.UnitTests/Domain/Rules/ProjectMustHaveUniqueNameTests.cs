using AwesomeAssertions;
using BauDoku.Projects.Domain.Rules;

namespace BauDoku.Projects.UnitTests.Domain.Rules;

public sealed class ProjectMustHaveUniqueNameTests
{
    [Fact]
    public void IsBroken_WhenNameAlreadyExists_ShouldReturnTrue()
    {
        var rule = new ProjectMustHaveUniqueName(nameAlreadyExists: true);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WhenNameDoesNotExist_ShouldReturnFalse()
    {
        var rule = new ProjectMustHaveUniqueName(nameAlreadyExists: false);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void Message_ShouldContainMeaningfulText()
    {
        var rule = new ProjectMustHaveUniqueName(nameAlreadyExists: true);

        rule.Message.Should().NotBeNullOrWhiteSpace();
        rule.Message.Should().Contain("Projekt");
    }
}
