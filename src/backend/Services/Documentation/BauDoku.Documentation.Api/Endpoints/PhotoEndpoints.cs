using System.Text.Json;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Dispatcher;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Responses;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;
using SmartSolutionsLab.BauDoku.Documentation.Api.Mapping;
using SmartSolutionsLab.BauDoku.Documentation.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Application.Queries;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;
using SmartSolutionsLab.BauDoku.Documentation.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SmartSolutionsLab.BauDoku.Documentation.Api.Endpoints;

public static class PhotoEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static IEndpointRouteBuilder MapPhotoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation")
            .WithTags("Photos")
            .RequireAuthorization(AuthPolicies.RequireInspector)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/installations/{installationId:guid}/photos", AddPhoto)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .DisableAntiforgery()
            .WithName("AddPhoto")
            .WithSummary("Foto zu einer Installation hinzufuegen")
            .ProducesValidationProblem();

        group.MapGet("/installations/{installationId:guid}/photos", ListPhotos)
            .WithName("ListPhotos")
            .WithSummary("Fotos einer Installation auflisten");

        group.MapGet("/photos/{photoId:guid}", GetPhoto)
            .WithName("GetPhoto")
            .WithSummary("Foto nach ID abrufen")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/installations/{installationId:guid}/photos/{photoId:guid}", RemovePhoto)
            .RequireAuthorization(AuthPolicies.RequireAdmin)
            .WithName("RemovePhoto")
            .WithSummary("Foto von einer Installation entfernen")
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Created<CreatedResponse>> AddPhoto(
        Guid installationId,
        IFormFile file,
        [FromForm] string? metadata,
        IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var request = GetRequestFromFormData(metadata);
        await using var stream = file.OpenReadStream();
        var command = request.ToCommand(installationId, file, stream);
        var photoId = await dispatcher.Send(command, cancellationToken);

        return TypedResults.Created($"/api/documentation/photos/{photoId.Value}", new CreatedResponse(photoId.Value));
    }

    private static AddPhotoRequest GetRequestFromFormData(string? metadata)
    {
        AddPhotoRequest? deserialized = null;

        if (metadata is not null)
        {
            deserialized = JsonSerializer.Deserialize<AddPhotoRequest>(metadata, JsonOptions);
        }

        return deserialized ?? new AddPhotoRequest(null, null, null, null, null);
    }

    private static async Task<Ok<IReadOnlyList<PhotoDto>>> ListPhotos(Guid installationId, IPhotoReadRepository photos, CancellationToken cancellationToken)
    {
        var photoList = await photos.ListByInstallationIdAsync(InstallationIdentifier.From(installationId), cancellationToken);

        return TypedResults.Ok(photoList);
    }

    private static async Task<Ok<PhotoDto>> GetPhoto(Guid photoId, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var query = new GetPhotoQuery(PhotoIdentifier.From(photoId));

        return TypedResults.Ok(await dispatcher.Query(query, cancellationToken));
    }

    private static async Task<NoContent> RemovePhoto(Guid photoId, Guid installationId, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = new RemovePhotoCommand(InstallationIdentifier.From(installationId), PhotoIdentifier.From(photoId));
        await dispatcher.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
