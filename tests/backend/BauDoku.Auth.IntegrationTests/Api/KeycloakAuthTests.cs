using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Auth.IntegrationTests.Fixtures;

namespace BauDoku.Auth.IntegrationTests.Api;

[Collection(KeycloakCollection.Name)]
public sealed class KeycloakAuthTests : IDisposable
{
    private readonly KeycloakFixture fixture;
    private readonly KeycloakApiFactory factory;
    private readonly HttpClient client;

    public KeycloakAuthTests(KeycloakFixture fixture)
    {
        this.fixture = fixture;
        factory = new KeycloakApiFactory(fixture);
        client = factory.CreateClient();
    }

    [Fact]
    public async Task ListProjects_WithAdminToken_Returns200()
    {
        var token = await fixture.GetTokenAsync("admin@test.de", "test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProject_WithUserToken_Returns201()
    {
        var token = await fixture.GetTokenAsync("user@test.de", "test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/projects", new
        {
            Name = "Keycloak Auth Test Project",
            Street = "Teststraße 1",
            City = "Berlin",
            ZipCode = "10115",
            ClientName = "Test Client"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task ListProjects_WithInspectorToken_Returns200()
    {
        var token = await fixture.GetTokenAsync("inspector@test.de", "test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProject_WithInspectorToken_Returns403()
    {
        var token = await fixture.GetTokenAsync("inspector@test.de", "test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/projects", new
        {
            Name = "Should Be Forbidden",
            Street = "Teststraße 1",
            City = "Berlin",
            ZipCode = "10115",
            ClientName = "Test Client"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ListProjects_WithoutToken_Returns401()
    {
        var response = await client.GetAsync("/api/projects");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListProjects_WithGarbageToken_Returns401()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "this.is.not.a.valid.jwt");

        var response = await client.GetAsync("/api/projects");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProject_WithAdminToken_Returns201()
    {
        var token = await fixture.GetTokenAsync("admin@test.de", "test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/projects", new
        {
            Name = "Admin Created Project",
            Street = "Adminstraße 1",
            City = "München",
            ZipCode = "80331",
            ClientName = "Admin Client"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }
}
