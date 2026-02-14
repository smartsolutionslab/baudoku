using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using BauDoku.E2E.SmokeTests.Fixtures;

namespace BauDoku.E2E.SmokeTests.Api;

[Collection(E2ECollection.Name)]
public sealed class FullWorkflowSmokeTest : IDisposable
{
    private readonly ProjectsApiFactory projectsFactory;
    private readonly DocumentationApiFactory documentationFactory;
    private readonly SyncApiFactory syncFactory;
    private readonly HttpClient projectsClient;
    private readonly HttpClient documentationClient;
    private readonly HttpClient syncClient;

    public FullWorkflowSmokeTest(E2EFixture fixture)
    {
        projectsFactory = new ProjectsApiFactory(fixture);
        documentationFactory = new DocumentationApiFactory(fixture);
        syncFactory = new SyncApiFactory(fixture);
        projectsClient = projectsFactory.CreateClient();
        documentationClient = documentationFactory.CreateClient();
        syncClient = syncFactory.CreateClient();
    }

    [Fact]
    public async Task FullWorkflow_ProjectToSync_ShouldSucceed()
    {
        // Step 1: Create a project
        var createProjectCommand = new
        {
            Name = "E2E Testprojekt",
            Street = "Teststraße 42",
            City = "Berlin",
            ZipCode = "10115",
            ClientName = "E2E GmbH"
        };

        var createProjectResponse = await projectsClient.PostAsJsonAsync("/api/projects", createProjectCommand);
        createProjectResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var projectResult = await createProjectResponse.Content.ReadFromJsonAsync<IdResponse>();
        projectResult.Should().NotBeNull();
        var projectId = projectResult!.Id;
        projectId.Should().NotBe(Guid.Empty);

        // Step 2: Verify project exists
        var getProjectResponse = await projectsClient.GetAsync($"/api/projects/{projectId}");
        getProjectResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var project = await getProjectResponse.Content.ReadFromJsonAsync<ProjectDto>();
        project.Should().NotBeNull();
        project!.Name.Should().Be("E2E Testprojekt");

        // Step 3: Add a zone to the project
        var addZoneRequest = new { Name = "Erdgeschoss", Type = "floor" };
        var addZoneResponse = await projectsClient.PostAsJsonAsync($"/api/projects/{projectId}/zones", addZoneRequest);
        addZoneResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Step 4: Create an installation in the Documentation BC
        var createInstallationCommand = new
        {
            ProjectId = projectId,
            Type = "cable_tray",
            Latitude = 52.5200,
            Longitude = 13.4050,
            Altitude = 34.0,
            HorizontalAccuracy = 3.5,
            GpsSource = "internal_gps",
            Description = "E2E Kabeltrasse"
        };

        var createInstallationResponse = await documentationClient.PostAsJsonAsync(
            "/api/documentation/installations", createInstallationCommand);
        createInstallationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var installationResult = await createInstallationResponse.Content.ReadFromJsonAsync<IdResponse>();
        installationResult.Should().NotBeNull();
        var installationId = installationResult!.Id;
        installationId.Should().NotBe(Guid.Empty);

        // Step 5: Add a photo to the installation (multipart/form-data)
        using var photoContent = new MultipartFormDataContent();
        var fileBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // minimal JPEG header
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        photoContent.Add(fileContent, "file", "e2e-photo.jpg");

        var addPhotoResponse = await documentationClient.PostAsync(
            $"/api/documentation/installations/{installationId}/photos", photoContent);
        addPhotoResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var photoResult = await addPhotoResponse.Content.ReadFromJsonAsync<IdResponse>();
        photoResult.Should().NotBeNull();
        photoResult!.Id.Should().NotBe(Guid.Empty);

        // Step 6: Record a measurement
        var recordMeasurementRequest = new
        {
            Type = "insulation_resistance",
            Value = 500.0,
            Unit = "MΩ"
        };

        var recordMeasurementResponse = await documentationClient.PostAsJsonAsync(
            $"/api/documentation/installations/{installationId}/measurements", recordMeasurementRequest);
        recordMeasurementResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var measurementResult = await recordMeasurementResponse.Content.ReadFromJsonAsync<IdResponse>();
        measurementResult.Should().NotBeNull();
        measurementResult!.Id.Should().NotBe(Guid.Empty);

        // Step 7: Verify the full installation with photo and measurement
        var getInstallationResponse = await documentationClient.GetAsync(
            $"/api/documentation/installations/{installationId}");
        getInstallationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var installation = await getInstallationResponse.Content.ReadFromJsonAsync<InstallationDto>();
        installation.Should().NotBeNull();
        installation!.ProjectId.Should().Be(projectId);
        installation.Type.Should().Be("cable_tray");
        installation.Photos.Should().HaveCount(1);
        installation.Measurements.Should().HaveCount(1);

        // Step 8: Sync the project as a delta via Sync BC
        var syncBatchCommand = new
        {
            DeviceId = "e2e-test-device",
            Deltas = new[]
            {
                new
                {
                    EntityType = "project",
                    EntityId = projectId,
                    Operation = "create",
                    BaseVersion = 0L,
                    Payload = $$$"""{"name":"E2E Testprojekt","city":"Berlin"}""",
                    Timestamp = DateTime.UtcNow
                }
            }
        };

        var syncBatchResponse = await syncClient.PostAsJsonAsync("/api/sync/batch", syncBatchCommand);
        syncBatchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var syncResult = await syncBatchResponse.Content.ReadFromJsonAsync<SyncBatchResultDto>();
        syncResult.Should().NotBeNull();
        syncResult!.AppliedCount.Should().BeGreaterThanOrEqualTo(1);

        // Step 9: Verify changes are visible to another device via delta query
        var getChangesResponse = await syncClient.GetAsync(
            "/api/sync/changes?deviceId=e2e-other-device");
        getChangesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var changes = await getChangesResponse.Content.ReadFromJsonAsync<ChangeSetResultDto>();
        changes.Should().NotBeNull();
        changes!.Changes.Should().NotBeEmpty();
    }

    public void Dispose()
    {
        projectsClient.Dispose();
        documentationClient.Dispose();
        syncClient.Dispose();
        projectsFactory.Dispose();
        documentationFactory.Dispose();
        syncFactory.Dispose();
    }

    // DTOs for deserialization
    private sealed record IdResponse(Guid Id);

    private sealed record ProjectDto(
        Guid Id,
        string Name,
        string Status,
        string Street,
        string City,
        string ZipCode,
        string ClientName,
        string? ClientEmail,
        string? ClientPhone,
        DateTime CreatedAt,
        IReadOnlyList<ZoneDto> Zones);

    private sealed record ZoneDto(Guid Id, string Name, string Type, Guid? ParentZoneId);

    private sealed record InstallationDto(
        Guid Id,
        Guid ProjectId,
        Guid? ZoneId,
        string Type,
        string Status,
        double Latitude,
        double Longitude,
        IReadOnlyList<PhotoDto> Photos,
        IReadOnlyList<MeasurementDto> Measurements);

    private sealed record PhotoDto(Guid Id, Guid InstallationId, string FileName);

    private sealed record MeasurementDto(Guid Id, Guid InstallationId, string Type, double Value, string Unit);

    private sealed record SyncBatchResultDto(
        Guid BatchId,
        int AppliedCount,
        int ConflictCount,
        List<ConflictDto> Conflicts);

    private sealed record ConflictDto(Guid Id, string EntityType, Guid EntityId);

    private sealed record ChangeSetResultDto(
        List<ServerDeltaDto> Changes,
        DateTime ServerTimestamp,
        bool HasMore);

    private sealed record ServerDeltaDto(
        string EntityType,
        Guid EntityId,
        string Operation,
        long Version,
        string Payload,
        DateTime Timestamp);
}
