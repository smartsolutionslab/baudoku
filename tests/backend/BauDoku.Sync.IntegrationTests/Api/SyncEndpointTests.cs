using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Sync.IntegrationTests.Fixtures;

namespace BauDoku.Sync.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class SyncEndpointTests : IDisposable
{
    private readonly SyncApiFactory _factory;
    private readonly HttpClient _client;

    public SyncEndpointTests(PostgreSqlFixture fixture)
    {
        _factory = new SyncApiFactory(fixture);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ProcessSyncBatch_WithValidData_ShouldReturn200()
    {
        var command = new
        {
            DeviceId = "test-device-001",
            Deltas = new[]
            {
                new
                {
                    EntityType = "project",
                    EntityId = Guid.NewGuid(),
                    Operation = "create",
                    BaseVersion = 0L,
                    Payload = """{"name":"Test"}""",
                    Timestamp = DateTime.UtcNow
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/sync/batch", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetChangesSince_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/sync/changes?deviceId=test-device");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetConflicts_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/sync/conflicts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
