using AwesomeAssertions;
using BauDoku.Projects.Domain.Rules;
using BauDoku.Projects.Domain.ValueObjects;
using BauDoku.Projects.UnitTests.Builders;

namespace BauDoku.Projects.UnitTests.Domain.Rules;

public sealed class ZoneNameMustBeUniqueWithinProjectTests
{
    [Fact]
    public void IsBroken_WhenNameExistsAtSameLevel_ShouldReturnTrue()
    {
        var project = new ProjectBuilder().Build();
        project.AddZone(ZoneIdentifier.New(), ZoneName.From("Erdgeschoss"), ZoneType.Floor);

        var rule = new ZoneNameMustBeUniqueWithinProject(
            project.Zones, ZoneName.From("Erdgeschoss"));

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WhenNameDoesNotExist_ShouldReturnFalse()
    {
        var project = new ProjectBuilder().Build();
        project.AddZone(ZoneIdentifier.New(), ZoneName.From("Erdgeschoss"), ZoneType.Floor);

        var rule = new ZoneNameMustBeUniqueWithinProject(
            project.Zones, ZoneName.From("Obergeschoss"));

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WhenNameExistsUnderDifferentParent_ShouldReturnFalse()
    {
        var project = new ProjectBuilder().Build();
        var parentId = ZoneIdentifier.New();
        project.AddZone(parentId, ZoneName.From("Gebäude A"), ZoneType.Building);
        project.AddZone(ZoneIdentifier.New(), ZoneName.From("Raum 1"), ZoneType.Room, parentId);

        var otherParentId = ZoneIdentifier.New();
        var rule = new ZoneNameMustBeUniqueWithinProject(
            project.Zones, ZoneName.From("Raum 1"), otherParentId);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WhenNameExistsUnderSameParent_ShouldReturnTrue()
    {
        var project = new ProjectBuilder().Build();
        var parentId = ZoneIdentifier.New();
        project.AddZone(parentId, ZoneName.From("Gebäude A"), ZoneType.Building);
        project.AddZone(ZoneIdentifier.New(), ZoneName.From("Raum 1"), ZoneType.Room, parentId);

        var rule = new ZoneNameMustBeUniqueWithinProject(
            project.Zones, ZoneName.From("Raum 1"), parentId);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WhenNoZonesExist_ShouldReturnFalse()
    {
        var project = new ProjectBuilder().Build();

        var rule = new ZoneNameMustBeUniqueWithinProject(
            project.Zones, ZoneName.From("Erdgeschoss"));

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void Message_ShouldContainZoneName()
    {
        var project = new ProjectBuilder().Build();

        var rule = new ZoneNameMustBeUniqueWithinProject(
            project.Zones, ZoneName.From("Erdgeschoss"));

        rule.Message.Should().Contain("Erdgeschoss");
    }
}
