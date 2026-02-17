using System.Net.Http.Json;
using System.Text.Json;
using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Projects.Infrastructure.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace BauDoku.Auth.IntegrationTests.Fixtures;

public sealed class KeycloakFixture : IAsyncLifetime
{
    private const int KeycloakPort = 8080;
    private const int ManagementPort = 9000;

    private readonly IContainer keycloak;
    private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public KeycloakFixture()
    {
        var realmPath = Path.Combine(AppContext.BaseDirectory, "keycloak", "test-realm.json");
        var realmBytes = File.ReadAllBytes(realmPath);

        keycloak = new ContainerBuilder("quay.io/keycloak/keycloak:26.2")
            .WithPortBinding(KeycloakPort, true)
            .WithPortBinding(ManagementPort, true)
            .WithResourceMapping(realmBytes, "/opt/keycloak/data/import/baudoku-test-realm.json")
            .WithCommand("start-dev", "--import-realm")
            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request =>
                    request.ForPath("/health/ready").ForPort(ManagementPort)))
            .Build();
    }

    public string KeycloakAuthority
    {
        get
        {
            var host = keycloak.Hostname;
            var port = keycloak.GetMappedPublicPort(KeycloakPort);
            return $"http://{host}:{port}/realms/baudoku-test";
        }
    }

    public string PostgresConnectionString => postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(keycloak.StartAsync(), postgres.StartAsync());

        await using var context = CreateProjectsDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task<string> GetTokenAsync(string username, string password)
    {
        using var httpClient = new HttpClient();
        var tokenEndpoint = $"{KeycloakAuthority}/protocol/openid-connect/token";

        var response = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "baudoku-test-client",
                ["username"] = username,
                ["password"] = password,
                ["scope"] = "openid",
            }));

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("access_token").GetString()!;
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            keycloak.DisposeAsync().AsTask(),
            postgres.DisposeAsync().AsTask());
    }

    private ProjectsDbContext CreateProjectsDbContext()
    {
        var options = new DbContextOptionsBuilder<ProjectsDbContext>()
            .UseNpgsql(PostgresConnectionString)
            .Options;

        var dispatcher = Substitute.For<IDispatcher>();
        return new ProjectsDbContext(options, dispatcher);
    }
}
