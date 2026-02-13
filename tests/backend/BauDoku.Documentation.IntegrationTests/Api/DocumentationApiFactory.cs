using BauDoku.Documentation.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Documentation.IntegrationTests.Api;

public sealed class DocumentationApiFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlFixture _fixture;

    public DocumentationApiFactory(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DocumentationDb", _fixture.ConnectionString);
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
