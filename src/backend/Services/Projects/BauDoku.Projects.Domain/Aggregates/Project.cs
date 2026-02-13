using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.Entities;
using BauDoku.Projects.Domain.Events;
using BauDoku.Projects.Domain.Rules;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Aggregates;

public sealed class Project : AggregateRoot<ProjectIdentifier>
{
    private readonly List<Zone> zones = [];

    public ProjectName Name { get; private set; } = default!;
    public ProjectStatus Status { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public ClientInfo Client { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<Zone> Zones => zones.AsReadOnly();

    private Project() { } // EF Core

    public static Project Create(ProjectIdentifier id, ProjectName name, Address address, ClientInfo client)
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

    public void AddZone(ZoneIdentifier zoneId, ZoneName name, ZoneType type, ZoneIdentifier? parentZoneIdentifier = null)
    {
        if (parentZoneIdentifier is not null && zones.All(z => z.Id != parentZoneIdentifier))
            throw new InvalidOperationException($"Elternzone {parentZoneIdentifier.Value} nicht gefunden.");

        CheckRule(new ZoneNameMustBeUniqueWithinProject(zones, name, parentZoneIdentifier));

        var zone = Zone.Create(zoneId, name, type, parentZoneIdentifier);
        zones.Add(zone);

        AddDomainEvent(new ZoneAdded(Id, zoneId, name, type, DateTime.UtcNow));
    }
}
