using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.Rules;

public sealed class CompletedInstallationCannotBeModifiedTests
{
    [Fact]
    public void IsBroken_WhenInProgress_ShouldReturnFalse()
    {
        var rule = new CompletedInstallationCannotBeModified(InstallationStatus.InProgress);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WhenCompleted_ShouldReturnTrue()
    {
        var rule = new CompletedInstallationCannotBeModified(InstallationStatus.Completed);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WhenInspected_ShouldReturnTrue()
    {
        var rule = new CompletedInstallationCannotBeModified(InstallationStatus.Inspected);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void Message_ShouldContainExpectedText()
    {
        var rule = new CompletedInstallationCannotBeModified(InstallationStatus.Completed);

        rule.Message.Should().Contain("abgeschlossene Installation");
    }
}
