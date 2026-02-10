using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.Entities;
using BauDoku.Projects.Domain.Events;
using BauDoku.Projects.Domain.Rules;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Aggregates;

public sealed class Project : AggregateRoot<ProjectId>
{
    private readonly List<Zone> _zones = [];

    public ProjectName Name { get; private set; } = default!;
    public ProjectStatus Status { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public ClientInfo Client { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<Zone> Zones => _zones.AsReadOnly();

    private Project() { } // EF Core

    public static Project Create(ProjectId id, ProjectName name, Address address, ClientInfo client)
    {
        var project = new Project
        {
            Id = id,
            Name = name,
            Status = ProjectStatus.Draft,
            Address = address,
            Client = client,
            CreatedAt = DateTime.UtcNow
        };

        project.AddDomainEvent(new ProjectCreated(id, name, DateTime.UtcNow));
        return project;
    }

    public void AddZone(ZoneId zoneId, ZoneName name, ZoneType type, ZoneId? parentZoneId = null)
    {
        CheckRule(new ZoneNameMustBeUniqueWithinProject(_zones, name));

        var zone = Zone.Create(zoneId, name, type, parentZoneId);
        _zones.Add(zone);

        AddDomainEvent(new ZoneAdded(Id, zoneId, name, type, DateTime.UtcNow));
    }
}
