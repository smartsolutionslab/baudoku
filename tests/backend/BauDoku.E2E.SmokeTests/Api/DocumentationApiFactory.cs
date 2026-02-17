extern alias DocumentationApi;

using BauDoku.E2E.SmokeTests.Fixtures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.E2E.SmokeTests.Api;

public sealed class DocumentationApiFactory(E2EFixture fixture) : WebApplicationFactory<DocumentationApi::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DocumentationDb", fixture.DocumentationConnectionString);
        builder.UseSetting("PhotoStorage:Provider", "Local");
        builder.UseSetting("PhotoStorage:LocalPath", Path.GetTempPath());

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });

        builder.UseEnvironment("Development");
    }
}
