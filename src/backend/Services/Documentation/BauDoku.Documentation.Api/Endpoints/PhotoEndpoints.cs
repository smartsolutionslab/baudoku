using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Documentation.Application.Commands.AddPhoto;
using BauDoku.Documentation.Application.Commands.RemovePhoto;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.GetPhoto;

namespace BauDoku.Documentation.Api.Endpoints;

public static class PhotoEndpoints
{
    public static void MapPhotoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation")
            .WithTags("Photos")
            .RequireAuthorization();

        group.MapPost("/installations/{installationId:guid}/photos", async (
            Guid installationId,
            IFormFile file,
            IDispatcher dispatcher,
            CancellationToken ct,
            string? photoType,
            string? caption,
            string? description,
            double? latitude,
            double? longitude,
            double? altitude,
            double? horizontalAccuracy,
            string? gpsSource) =>
        {
            await using var stream = file.OpenReadStream();

            var command = new AddPhotoCommand(
                installationId,
                file.FileName,
                file.ContentType,
                file.Length,
                photoType ?? "other",
                caption,
                description,
                latitude,
                longitude,
                altitude,
                horizontalAccuracy,
                gpsSource,
                stream);

            var photoId = await dispatcher.Send(command, ct);
            return Results.Created($"/api/documentation/photos/{photoId}", new { id = photoId });
        }).DisableAntiforgery();

        group.MapGet("/installations/{installationId:guid}/photos", async (
            Guid installationId,
            IPhotoReadRepository photoReadRepository,
            CancellationToken ct) =>
        {
            var photos = await photoReadRepository.ListByInstallationIdAsync(installationId, ct);
            return Results.Ok(photos);
        });

        group.MapGet("/photos/{photoId:guid}", async (
            Guid photoId,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var query = new GetPhotoQuery(photoId);
            var result = await dispatcher.Query(query, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        group.MapDelete("/photos/{photoId:guid}", async (
            Guid photoId,
            Guid installationId,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new RemovePhotoCommand(installationId, photoId);
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        });
    }
}
