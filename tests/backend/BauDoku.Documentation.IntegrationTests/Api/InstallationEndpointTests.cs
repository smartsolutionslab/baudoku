using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Documentation.IntegrationTests.Fixtures;

namespace BauDoku.Documentation.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class InstallationEndpointTests : IDisposable
{
    private readonly DocumentationApiFactory factory;
    private readonly HttpClient client;

    public InstallationEndpointTests(PostgreSqlFixture fixture)
    {
        factory = new DocumentationApiFactory(fixture);
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateInstallation_WithValidData_ShouldReturn201()
    {
        var command = new
        {
            ProjectId = Guid.NewGuid(),
            Type = "cable_tray",
            Latitude = 48.1351,
            Longitude = 11.5820,
            Altitude = 520.0,
            HorizontalAccuracy = 3.5,
            GpsSource = "internal_gps",
            Description = "Kabeltrasse Test"
        };

        var response = await client.PostAsJsonAsync("/api/documentation/installations", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task ListInstallations_ShouldReturn200()
    {
        var response = await client.GetAsync("/api/documentation/installations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetInstallation_WithExistingId_ShouldReturn200()
    {
        // Create first
        var command = new
        {
            ProjectId = Guid.NewGuid(),
            Type = "junction_box",
            Latitude = 48.0,
            Longitude = 11.0,
            HorizontalAccuracy = 5.0,
            GpsSource = "internal_gps"
        };
        var createResponse = await client.PostAsJsonAsync("/api/documentation/installations", command);
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var response = await client.GetAsync($"/api/documentation/installations/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetInstallation_WithNonExistentId_ShouldReturn404()
    {
        var response = await client.GetAsync($"/api/documentation/installations/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateInstallation_WithValidData_ShouldReturn204()
    {
        // Create first
        var createCommand = new
        {
            ProjectId = Guid.NewGuid(),
            Type = "cable_tray",
            Latitude = 48.0,
            Longitude = 11.0,
            HorizontalAccuracy = 5.0,
            GpsSource = "internal_gps"
        };
        var createResponse = await client.PostAsJsonAsync("/api/documentation/installations", createCommand);
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        // Update
        var updateRequest = new
        {
            Description = "Aktualisierte Beschreibung",
            DepthMm = 450
        };
        var response = await client.PutAsJsonAsync(
            $"/api/documentation/installations/{created!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateInstallation_WithNonExistentId_ShouldReturn404()
    {
        var updateRequest = new
        {
            Description = "Test"
        };
        var response = await client.PutAsJsonAsync(
            $"/api/documentation/installations/{Guid.NewGuid()}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }

    private sealed record IdResponse(Guid Id);
}
