using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Projects.IntegrationTests.Fixtures;

namespace BauDoku.Projects.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProjectEndpointTests : IDisposable
{
    private readonly ProjectsApiFactory _factory;
    private readonly HttpClient _client;

    public ProjectEndpointTests(PostgreSqlFixture fixture)
    {
        _factory = new ProjectsApiFactory(fixture);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateProject_WithValidData_ShouldReturn201()
    {
        var command = new
        {
            Name = "Testprojekt API",
            Street = "Teststraße 1",
            City = "Berlin",
            ZipCode = "10115",
            ClientName = "Test GmbH"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task ListProjects_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProject_WithExistingId_ShouldReturn200()
    {
        // Create a project first
        var command = new
        {
            Name = "Projekt zum Laden",
            Street = "Hauptstraße 5",
            City = "München",
            ZipCode = "80331",
            ClientName = "Kunde"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/projects", command);
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.GetAsync($"/api/projects/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProject_WithNonExistentId_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddZone_WithValidData_ShouldReturn204()
    {
        // Create a project first
        var command = new
        {
            Name = "Projekt mit Zonen",
            Street = "Zonenstraße 1",
            City = "Hamburg",
            ZipCode = "20095",
            ClientName = "Zonen GmbH"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/projects", command);
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var zoneRequest = new { Name = "Erdgeschoss", Type = "floor" };
        var response = await _client.PostAsJsonAsync($"/api/projects/{created!.Id}/zones", zoneRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private sealed record IdResponse(Guid Id);
}
