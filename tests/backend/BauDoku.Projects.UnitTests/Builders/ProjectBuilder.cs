using BauDoku.Projects.Domain;

namespace BauDoku.Projects.UnitTests.Builders;

internal sealed class ProjectBuilder
{
    private ProjectIdentifier id = ProjectIdentifier.New();
    private ProjectName name = ProjectName.From("Testprojekt");
    private Address address = Address.Create(Street.From("Musterstraße 1"), City.From("Berlin"), ZipCode.From("10115"));
    private ClientInfo client = ClientInfo.Create(ClientName.From("Max Mustermann"), EmailAddress.From("max@example.com"), PhoneNumber.From("+49 30 12345"));

    public ProjectBuilder WithId(ProjectIdentifier value) { id = value; return this; }
    public ProjectBuilder WithName(ProjectName value) { name = value; return this; }
    public ProjectBuilder WithAddress(Address value) { address = value; return this; }
    public ProjectBuilder WithClient(ClientInfo value) { client = value; return this; }

    public Project Build() => Project.Create(id, name, address, client);
}
