using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Builders;

internal sealed class ProjectBuilder
{
    private ProjectIdentifier id = ProjectIdentifier.New();
    private ProjectName name = ProjectName.From("Testprojekt");
    private Address address = Address.Create("Musterstraße 1", "Berlin", "10115");
    private ClientInfo client = ClientInfo.Create("Max Mustermann", "max@example.com", "+49 30 12345");

    public ProjectBuilder WithId(ProjectIdentifier value) { id = value; return this; }
    public ProjectBuilder WithName(ProjectName value) { name = value; return this; }
    public ProjectBuilder WithAddress(Address value) { address = value; return this; }
    public ProjectBuilder WithClient(ClientInfo value) { client = value; return this; }

    public Project Build() => Project.Create(id, name, address, client);
}
