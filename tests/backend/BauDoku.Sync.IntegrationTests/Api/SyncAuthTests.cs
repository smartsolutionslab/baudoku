using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Sync.IntegrationTests.Fixtures;

namespace BauDoku.Sync.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class SyncAuthTests : IDisposable
{
    private readonly SyncApiFactory _factory;
    private readonly HttpClient _client;

    public SyncAuthTests(PostgreSqlFixture fixture)
    {
        _factory = new SyncApiFactory(fixture);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ProcessSyncBatch_WithUserRole_Returns200()
    {
        TestAuthHandler.Roles = ["user"];
        try
        {
            var response = await _client.PostAsJsonAsync("/api/sync/batch", new
            {
                DeviceId = "test-device-auth",
                Deltas = new[]
                {
                    new
                    {
                        EntityType = "project",
                        EntityId = Guid.NewGuid(),
                        Operation = "create",
                        BaseVersion = 0L,
                        Payload = """{"name":"Auth Test"}""",
                        Timestamp = DateTime.UtcNow
                    }
                }
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    [Fact]
    public async Task ResolveConflict_WithUserRole_Returns403()
    {
        TestAuthHandler.Roles = ["user"];
        try
        {
            var conflictId = Guid.NewGuid();

            var response = await _client.PostAsJsonAsync($"/api/sync/conflicts/{conflictId}/resolve", new
            {
                Strategy = "server_wins",
                MergedPayload = (string?)null
            });

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    [Fact]
    public async Task GetChanges_WithInspectorRole_Returns200()
    {
        TestAuthHandler.Roles = ["inspector"];
        try
        {
            var response = await _client.GetAsync("/api/sync/changes?deviceId=test-device-auth");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
