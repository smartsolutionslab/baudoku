extern alias ProjectsApi;

using BauDoku.E2E.SmokeTests.Fixtures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.E2E.SmokeTests.Api;

public sealed class ProjectsApiFactory(E2EFixture fixture) : WebApplicationFactory<ProjectsApi::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:ProjectsDb", fixture.ProjectsConnectionString);

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });

        builder.UseEnvironment("Development");
    }
}
