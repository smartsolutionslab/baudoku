using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.Events;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.Aggregates;

public sealed class ProjectTests
{
    private static Project CreateValidProject()
    {
        return Project.Create(
            ProjectId.New(),
            new ProjectName("Testprojekt"),
            new Address("Musterstraße 1", "Berlin", "10115"),
            new ClientInfo("Max Mustermann", "max@example.com", "+49 30 12345"));
    }

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var id = ProjectId.New();
        var name = new ProjectName("Neues Projekt");
        var address = new Address("Hauptstraße 5", "München", "80331");
        var client = new ClientInfo("Firma GmbH");

        var project = Project.Create(id, name, address, client);

        project.Id.Should().Be(id);
        project.Name.Should().Be(name);
        project.Status.Should().Be(ProjectStatus.Draft);
        project.Address.Should().Be(address);
        project.Client.Should().Be(client);
        project.Zones.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldRaiseProjectCreatedEvent()
    {
        var project = CreateValidProject();

        project.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProjectCreated>();
    }

    [Fact]
    public void AddZone_ShouldAddZoneToProject()
    {
        var project = CreateValidProject();
        var zoneId = ZoneId.New();
        var zoneName = new ZoneName("Erdgeschoss");
        var zoneType = ZoneType.Floor;

        project.AddZone(zoneId, zoneName, zoneType);

        project.Zones.Should().ContainSingle();
        project.Zones[0].Name.Should().Be(zoneName);
        project.Zones[0].Type.Should().Be(zoneType);
    }

    [Fact]
    public void AddZone_ShouldRaiseZoneAddedEvent()
    {
        var project = CreateValidProject();
        project.ClearDomainEvents();

        project.AddZone(ZoneId.New(), new ZoneName("Keller"), ZoneType.Floor);

        project.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ZoneAdded>();
    }

    [Fact]
    public void AddZone_WithDuplicateName_ShouldThrowBusinessRuleException()
    {
        var project = CreateValidProject();
        var zoneName = new ZoneName("Erdgeschoss");

        project.AddZone(ZoneId.New(), zoneName, ZoneType.Floor);

        var act = () => project.AddZone(ZoneId.New(), zoneName, ZoneType.Room);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void AddZone_WithParentZoneId_ShouldSetParent()
    {
        var project = CreateValidProject();
        var parentId = ZoneId.New();
        project.AddZone(parentId, new ZoneName("Gebäude A"), ZoneType.Building);

        var childId = ZoneId.New();
        project.AddZone(childId, new ZoneName("Erdgeschoss"), ZoneType.Floor, parentId);

        project.Zones.Should().HaveCount(2);
        project.Zones[1].ParentZoneId.Should().Be(parentId);
    }
}
