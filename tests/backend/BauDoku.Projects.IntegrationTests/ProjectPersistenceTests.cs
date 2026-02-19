using AwesomeAssertions;
using BauDoku.Projects.Domain;
using BauDoku.Projects.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProjectPersistenceTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task CreateProject_ShouldPersistAndLoad()
    {
        // Arrange
        var projectId = ProjectIdentifier.New();
        var project = Project.Create(
            projectId,
            ProjectName.From("Testprojekt Persistence"),
            Address.Create(Street.From("Berliner Str. 1"), City.From("Hamburg"), ZipCode.From("20095")),
            ClientInfo.Create(ClientName.From("Testfirma GmbH"), EmailAddress.From("test@example.com")));

        // Act
        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Projects.Add(project);
            await writeContext.SaveChangesAsync();
        }

        // Assert
        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Projects
                .Include(p => p.Zones)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            loaded.Should().NotBeNull();
            loaded!.Name.Value.Should().Be("Testprojekt Persistence");
            loaded.Status.Should().Be(ProjectStatus.Draft);
            loaded.Address.Street.Value.Should().Be("Berliner Str. 1");
            loaded.Address.City.Value.Should().Be("Hamburg");
            loaded.Client.Name.Value.Should().Be("Testfirma GmbH");
            loaded.Client.Email!.Value.Should().Be("test@example.com");
            loaded.Zones.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task AddZone_ShouldPersistWithHierarchy()
    {
        // Arrange
        var projectId = ProjectIdentifier.New();
        var project = Project.Create(
            projectId,
            ProjectName.From("Zonenprojekt"),
            Address.Create(Street.From("Hauptstraße 10"), City.From("München"), ZipCode.From("80331")),
            ClientInfo.Create(ClientName.From("Bau AG")));

        var buildingId = ZoneIdentifier.New();
        project.AddZone(buildingId, ZoneName.From("Gebäude A"), ZoneType.Building);
        project.AddZone(ZoneIdentifier.New(), ZoneName.From("Erdgeschoss"), ZoneType.Floor, buildingId);

        // Act
        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Projects.Add(project);
            await writeContext.SaveChangesAsync();
        }

        // Assert
        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Projects
                .Include(p => p.Zones)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            loaded.Should().NotBeNull();
            loaded!.Zones.Should().HaveCount(2);

            var building = loaded.Zones.First(z => z.Type == ZoneType.Building);
            building.Name.Value.Should().Be("Gebäude A");
            building.ParentZoneIdentifier.Should().BeNull();

            var floor = loaded.Zones.First(z => z.Type == ZoneType.Floor);
            floor.Name.Value.Should().Be("Erdgeschoss");
            floor.ParentZoneIdentifier.Should().Be(buildingId);
        }
    }
}
