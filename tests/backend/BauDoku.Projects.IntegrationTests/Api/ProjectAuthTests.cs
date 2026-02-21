using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Projects.IntegrationTests.Fixtures;

namespace BauDoku.Projects.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProjectAuthTests : IDisposable
{
    private readonly ProjectsApiFactory factory;
    private readonly HttpClient client;

    public ProjectAuthTests(PostgreSqlFixture fixture)
    {
        factory = new ProjectsApiFactory(fixture);
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProject_WithUserRole_Returns201()
    {
        TestAuthHandler.Roles = ["user"];
        try
        {
            var response = await client.PostAsJsonAsync("/api/projects", new
            {
                Name = "Auth Test Project",
                Street = "Teststraße 1",
                City = "Berlin",
                ZipCode = "10115",
                ClientName = "Test Client"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    [Fact]
    public async Task CreateProject_WithInspectorRole_Returns403()
    {
        TestAuthHandler.Roles = ["inspector"];
        try
        {
            var response = await client.PostAsJsonAsync("/api/projects", new
            {
                Name = "Auth Test Project",
                Street = "Teststraße 1",
                City = "Berlin",
                ZipCode = "10115",
                ClientName = "Test Client"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    [Fact]
    public async Task ListProjects_WithInspectorRole_Returns200()
    {
        TestAuthHandler.Roles = ["inspector"];
        try
        {
            var response = await client.GetAsync("/api/projects");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        finally
        {
            TestAuthHandler.Roles = ["user", "admin"];
        }
    }

    [Fact]
    public async Task ListProjects_WithoutAuthentication_Returns401()
    {
        TestAuthHandler.IsAuthenticated = false;
        try
        {
            var response = await client.GetAsync("/api/projects");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        finally
        {
            TestAuthHandler.IsAuthenticated = true;
        }
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }
}
