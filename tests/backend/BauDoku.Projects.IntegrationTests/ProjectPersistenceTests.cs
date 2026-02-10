using AwesomeAssertions;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;
using BauDoku.Projects.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProjectPersistenceTests
{
    private readonly PostgreSqlFixture _fixture;

    public ProjectPersistenceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateProject_ShouldPersistAndLoad()
    {
        // Arrange
        var projectId = ProjectId.New();
        var project = Project.Create(
            projectId,
            new ProjectName("Testprojekt Persistence"),
            new Address("Berliner Str. 1", "Hamburg", "20095"),
            new ClientInfo("Testfirma GmbH", "test@example.com"));

        // Act
        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Projects.Add(project);
            await writeContext.SaveChangesAsync();
        }

        // Assert
        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.Projects
                .Include(p => p.Zones)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            loaded.Should().NotBeNull();
            loaded!.Name.Value.Should().Be("Testprojekt Persistence");
            loaded.Status.Should().Be(ProjectStatus.Draft);
            loaded.Address.Street.Should().Be("Berliner Str. 1");
            loaded.Address.City.Should().Be("Hamburg");
            loaded.Client.Name.Should().Be("Testfirma GmbH");
            loaded.Client.Email.Should().Be("test@example.com");
            loaded.Zones.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task AddZone_ShouldPersistWithHierarchy()
    {
        // Arrange
        var projectId = ProjectId.New();
        var project = Project.Create(
            projectId,
            new ProjectName("Zonenprojekt"),
            new Address("Hauptstraße 10", "München", "80331"),
            new ClientInfo("Bau AG"));

        var buildingId = ZoneId.New();
        project.AddZone(buildingId, new ZoneName("Gebäude A"), ZoneType.Building);
        project.AddZone(ZoneId.New(), new ZoneName("Erdgeschoss"), ZoneType.Floor, buildingId);

        // Act
        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Projects.Add(project);
            await writeContext.SaveChangesAsync();
        }

        // Assert
        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.Projects
                .Include(p => p.Zones)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            loaded.Should().NotBeNull();
            loaded!.Zones.Should().HaveCount(2);

            var building = loaded.Zones.First(z => z.Type == ZoneType.Building);
            building.Name.Value.Should().Be("Gebäude A");
            building.ParentZoneId.Should().BeNull();

            var floor = loaded.Zones.First(z => z.Type == ZoneType.Floor);
            floor.Name.Value.Should().Be("Erdgeschoss");
            floor.ParentZoneId.Should().Be(buildingId);
        }
    }
}
