using BauDoku.Auth.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BauDoku.Auth.IntegrationTests.Api;

public sealed class KeycloakApiFactory(KeycloakFixture fixture) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:ProjectsDb", fixture.PostgresConnectionString);
        builder.UseSetting("Authentication:Keycloak:Authority", fixture.KeycloakAuthority);
        builder.UseSetting("Authentication:Keycloak:Audience", "baudoku-api");
        builder.UseEnvironment("Development");
    }
}
