using AwesomeAssertions;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ProjectNameTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var name = ProjectName.From("Baustelle Musterstraße");

        name.Value.Should().Be("Baustelle Musterstraße");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string? value)
    {
        var act = () => ProjectName.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', ProjectName.MaxLength + 1);

        var act = () => ProjectName.From(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', ProjectName.MaxLength);

        var name = ProjectName.From(maxName);

        name.Value.Should().HaveLength(ProjectName.MaxLength);
    }
}
