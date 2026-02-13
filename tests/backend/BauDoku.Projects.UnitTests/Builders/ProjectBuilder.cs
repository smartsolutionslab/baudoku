using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Builders;

internal sealed class ProjectBuilder
{
    private ProjectId _id = ProjectId.New();
    private ProjectName _name = new("Testprojekt");
    private Address _address = new("Musterstraße 1", "Berlin", "10115");
    private ClientInfo _client = new("Max Mustermann", "max@example.com", "+49 30 12345");

    public ProjectBuilder WithId(ProjectId id) { _id = id; return this; }
    public ProjectBuilder WithName(ProjectName name) { _name = name; return this; }
    public ProjectBuilder WithAddress(Address address) { _address = address; return this; }
    public ProjectBuilder WithClient(ClientInfo client) { _client = client; return this; }

    public Project Build() => Project.Create(_id, _name, _address, _client);
}
