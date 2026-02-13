using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class MeasurementIdentifierTests
{
    [Fact]
    public void From_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = MeasurementIdentifier.From(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_WithEmptyGuid_ShouldThrow()
    {
        var act = () => MeasurementIdentifier.From(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldCreateUniqueId()
    {
        var id1 = MeasurementIdentifier.New();
        var id2 = MeasurementIdentifier.New();

        id1.Value.Should().NotBe(Guid.Empty);
        id1.Should().NotBe(id2);
    }
}
