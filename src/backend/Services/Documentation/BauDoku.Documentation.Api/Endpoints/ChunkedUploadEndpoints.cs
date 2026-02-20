using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Infrastructure.Auth;
using BauDoku.Documentation.Application.Commands.CompleteChunkedUpload;
using BauDoku.Documentation.Application.Commands.InitChunkedUpload;
using BauDoku.Documentation.Application.Commands.UploadChunk;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Api.Endpoints;

public static class ChunkedUploadEndpoints
{
    public static void MapChunkedUploadEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation/uploads").WithTags("Chunked Upload");

        group.MapPost("/init", async (
            InitChunkedUploadCommand command,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var sessionId = await dispatcher.Send(command, ct);
            return Results.Ok(new CreatedResponse(sessionId));
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("InitChunkedUpload")
        .WithSummary("Chunked-Upload-Sitzung initialisieren")
        .Produces<CreatedResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem();

        group.MapPost("/{sessionId:guid}/chunks/{index:int}", async (
            Guid sessionId,
            int index,
            HttpRequest httpRequest,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new UploadChunkCommand(UploadSessionIdentifier.From(sessionId), index, httpRequest.Body);
            await dispatcher.Send(command, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .DisableAntiforgery()
        .WithName("UploadChunk")
        .WithSummary("Einen Chunk hochladen")
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/{sessionId:guid}/complete", async (
            Guid sessionId,
            IDispatcher dispatcher,
            CancellationToken ct) =>
        {
            var command = new CompleteChunkedUploadCommand(UploadSessionIdentifier.From(sessionId));
            var photoId = await dispatcher.Send(command, ct);
            return Results.Created($"/api/documentation/photos/{photoId}", new CreatedResponse(photoId));
        })
        .RequireAuthorization(AuthPolicies.RequireUser)
        .WithName("CompleteChunkedUpload")
        .WithSummary("Chunked-Upload abschliessen und Foto erstellen")
        .Produces<CreatedResponse>(StatusCodes.Status201Created);
    }
}
