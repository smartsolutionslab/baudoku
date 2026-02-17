using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Documentation.IntegrationTests.Fixtures;

namespace BauDoku.Documentation.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class DocumentationAuthTests : IDisposable
{
    private readonly DocumentationApiFactory _factory;
    private readonly HttpClient _client;

    public DocumentationAuthTests(PostgreSqlFixture fixture)
    {
        _factory = new DocumentationApiFactory(fixture);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task DocumentInstallation_WithUserRole_Returns201()
    {
        TestAuthHandler.Roles = ["user"];
        try
        {
            var response = await _client.PostAsJsonAsync("/api/documentation/installations", new
            {
                ProjectId = Guid.NewGuid(),
                Type = "cable_tray",
                Latitude = 52.52,
                Longitude = 13.405,
                Altitude = 34.0,
                HorizontalAccuracy = 3.5,
                GpsSource = "internal_gps",
                Description = "Auth test installation"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    [Fact]
    public async Task DocumentInstallation_WithInspectorRole_Returns403()
    {
        TestAuthHandler.Roles = ["inspector"];
        try
        {
            var response = await _client.PostAsJsonAsync("/api/documentation/installations", new
            {
                ProjectId = Guid.NewGuid(),
                Type = "cable_tray",
                Latitude = 52.52,
                Longitude = 13.405,
                Altitude = 34.0,
                HorizontalAccuracy = 3.5,
                GpsSource = "internal_gps",
                Description = "Auth test installation"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    [Fact]
    public async Task RemovePhoto_WithUserRole_Returns403()
    {
        TestAuthHandler.Roles = ["user"];
        try
        {
            var installationId = Guid.NewGuid();
            var photoId = Guid.NewGuid();

            var response = await _client.DeleteAsync(
                $"/api/documentation/installations/{installationId}/photos/{photoId}");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    [Fact]
    public async Task ListInstallations_WithoutAuthentication_Returns401()
    {
        TestAuthHandler.IsAuthenticated = false;
        try
        {
            var response = await _client.GetAsync("/api/documentation/installations");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        finally
        {
            TestAuthHandler.IsAuthenticated = true;
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
