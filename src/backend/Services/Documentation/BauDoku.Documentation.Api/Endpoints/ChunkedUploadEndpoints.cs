using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Responses;
using BauDoku.BuildingBlocks.Auth;
using BauDoku.Documentation.Api.Mapping;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BauDoku.Documentation.Api.Endpoints;

public static class ChunkedUploadEndpoints
{
    public static IEndpointRouteBuilder MapChunkedUploadEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documentation/uploads")
            .WithTags("Chunked Upload")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/init", InitChunkedUpload)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("InitChunkedUpload")
            .WithSummary("Chunked-Upload-Sitzung initialisieren")
            .ProducesValidationProblem();

        group.MapPost("/{sessionId:guid}/chunks/{index:int}", UploadChunk)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .DisableAntiforgery()
            .WithName("UploadChunk")
            .WithSummary("Einen Chunk hochladen");

        group.MapPost("/{sessionId:guid}/complete", CompleteChunkedUpload)
            .RequireAuthorization(AuthPolicies.RequireUser)
            .WithName("CompleteChunkedUpload")
            .WithSummary("Chunked-Upload abschliessen und Foto erstellen");

        return app;
    }

    private static async Task<Ok<CreatedResponse>> InitChunkedUpload(InitChunkedUploadRequest request, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var sessionId = await dispatcher.Send(command, cancellationToken);

        return TypedResults.Ok(new CreatedResponse(sessionId.Value));
    }

    private static async Task<NoContent> UploadChunk(Guid sessionId, int index, HttpRequest httpRequest, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = new UploadChunkCommand(
            UploadSessionIdentifier.From(sessionId),
            ChunkIndex.From(index),
            httpRequest.Body);
        await dispatcher.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<Created<CreatedResponse>> CompleteChunkedUpload(Guid sessionId, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var command = new CompleteChunkedUploadCommand(UploadSessionIdentifier.From(sessionId));
        var photoId = await dispatcher.Send(command, cancellationToken);

        return TypedResults.Created($"/api/documentation/photos/{photoId.Value}", new CreatedResponse(photoId.Value));
    }
}
