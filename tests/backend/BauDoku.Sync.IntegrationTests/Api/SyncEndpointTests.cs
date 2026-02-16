using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Sync.IntegrationTests.Fixtures;

namespace BauDoku.Sync.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class SyncEndpointTests : IDisposable
{
    private readonly SyncApiFactory factory;
    private readonly HttpClient client;

    public SyncEndpointTests(PostgreSqlFixture fixture)
    {
        factory = new SyncApiFactory(fixture);
        client = factory.CreateClient();
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

        var response = await client.PostAsJsonAsync("/api/sync/batch", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetChangesSince_ShouldReturn200()
    {
        var response = await client.GetAsync("/api/sync/changes?deviceId=test-device");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetConflicts_ShouldReturn200()
    {
        var response = await client.GetAsync("/api/sync/conflicts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResolveConflict_WithValidData_ShouldReturn204()
    {
        var entityId = Guid.NewGuid();

        // First batch: create the entity (version 0 → 1)
        var firstBatch = new
        {
            DeviceId = "device-A",
            Deltas = new[]
            {
                new
                {
                    EntityType = "project",
                    EntityId = entityId,
                    Operation = "create",
                    BaseVersion = 0L,
                    Payload = """{"name":"Original"}""",
                    Timestamp = DateTime.UtcNow
                }
            }
        };
        var firstResponse = await client.PostAsJsonAsync("/api/sync/batch", firstBatch);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Second batch from different device with stale version → creates conflict
        var conflictBatch = new
        {
            DeviceId = "device-B",
            Deltas = new[]
            {
                new
                {
                    EntityType = "project",
                    EntityId = entityId,
                    Operation = "update",
                    BaseVersion = 0L, // stale — server is already at version 1
                    Payload = """{"name":"Conflict"}""",
                    Timestamp = DateTime.UtcNow
                }
            }
        };
        var conflictResponse = await client.PostAsJsonAsync("/api/sync/batch", conflictBatch);
        conflictResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var batchResult = await conflictResponse.Content.ReadFromJsonAsync<BatchResultDto>();
        batchResult!.ConflictCount.Should().BeGreaterThan(0);
        var conflictId = batchResult.Conflicts[0].Id;

        // Resolve the conflict
        var resolveRequest = new
        {
            Strategy = "server_wins",
            MergedPayload = (string?)null
        };
        var resolveResponse = await client.PostAsJsonAsync(
            $"/api/sync/conflicts/{conflictId}/resolve", resolveRequest);

        resolveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }

    private sealed record BatchResultDto(Guid BatchId, int AppliedCount, int ConflictCount, List<ConflictItemDto> Conflicts);
    private sealed record ConflictItemDto(Guid Id, string EntityType, Guid EntityId, string ClientPayload, string ServerPayload, long ClientVersion, long ServerVersion, string Status, DateTime DetectedAt);
}
