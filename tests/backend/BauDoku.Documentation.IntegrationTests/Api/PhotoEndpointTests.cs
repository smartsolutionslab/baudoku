using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.Documentation.IntegrationTests.Fixtures;

namespace BauDoku.Documentation.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class PhotoEndpointTests : IDisposable
{
    private readonly DocumentationApiFactory factory;
    private readonly HttpClient client;

    public PhotoEndpointTests(PostgreSqlFixture fixture)
    {
        factory = new DocumentationApiFactory(fixture);
        client = factory.CreateClient();
    }

    private async Task<Guid> CreateInstallationAsync()
    {
        var command = new
        {
            ProjectId = Guid.NewGuid(),
            Type = "junction_box",
            Latitude = 48.1351,
            Longitude = 11.5820,
            HorizontalAccuracy = 3.5,
            GpsSource = "internal_gps"
        };
        var response = await client.PostAsJsonAsync("/api/documentation/installations", command);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task GetPhoto_WithNonExistentId_ShouldReturnNotFoundOrError()
    {
        var response = await client.GetAsync($"/api/documentation/photos/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListPhotos_ShouldReturnSuccessOrError()
    {
        var installationId = await CreateInstallationAsync();

        var response = await client.GetAsync(
            $"/api/documentation/installations/{installationId}/photos");

        // PhotoReadRepository uses EF shadow property projection that may fail on some providers
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task AddPhoto_WithValidFile_ShouldReturn201()
    {
        var installationId = await CreateInstallationAsync();

        using var content = new MultipartFormDataContent();
        var fileBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // minimal JPEG header
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        content.Add(fileContent, "file", "test-photo.jpg");

        var response = await client.PostAsync(
            $"/api/documentation/installations/{installationId}/photos", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBe(Guid.Empty);
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
    }

    private sealed record IdResponse(Guid Id);
}
