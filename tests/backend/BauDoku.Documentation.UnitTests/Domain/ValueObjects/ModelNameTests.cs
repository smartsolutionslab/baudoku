using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ModelNameTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var model = new ModelName("VZ312N");

        model.Value.Should().Be("VZ312N");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string? value)
    {
        var act = () => new ModelName(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', ModelName.MaxLength + 1);

        var act = () => new ModelName(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', ModelName.MaxLength);

        var model = new ModelName(maxName);

        model.Value.Should().HaveLength(ModelName.MaxLength);
    }
}
