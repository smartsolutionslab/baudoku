using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Documentation.IntegrationTests.Fixtures;

namespace BauDoku.Documentation.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class MeasurementEndpointTests : IDisposable
{
    private readonly DocumentationApiFactory _factory;
    private readonly HttpClient _client;

    public MeasurementEndpointTests(PostgreSqlFixture fixture)
    {
        _factory = new DocumentationApiFactory(fixture);
        _client = _factory.CreateClient();
    }

    private async Task<Guid> CreateInstallationAsync()
    {
        var command = new
        {
            ProjectId = Guid.NewGuid(),
            Type = "cable_tray",
            Latitude = 48.1351,
            Longitude = 11.5820,
            HorizontalAccuracy = 3.5,
            GpsSource = "internal_gps"
        };
        var response = await _client.PostAsJsonAsync("/api/documentation/installations", command);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task RecordMeasurement_WithValidData_ShouldReturn201()
    {
        var installationId = await CreateInstallationAsync();

        var request = new
        {
            Type = "insulation_resistance",
            Value = 500.0,
            Unit = "MΩ"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/documentation/installations/{installationId}/measurements", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetMeasurements_ShouldReturn200()
    {
        var installationId = await CreateInstallationAsync();

        var response = await _client.GetAsync(
            $"/api/documentation/installations/{installationId}/measurements");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveMeasurement_ShouldReturn204()
    {
        var installationId = await CreateInstallationAsync();

        // Record a measurement first
        var request = new
        {
            Type = "insulation_resistance",
            Value = 500.0,
            Unit = "MΩ"
        };
        var createResponse = await _client.PostAsJsonAsync(
            $"/api/documentation/installations/{installationId}/measurements", request);
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        // Delete it
        var response = await _client.DeleteAsync(
            $"/api/documentation/installations/{installationId}/measurements/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private sealed record IdResponse(Guid Id);
}
