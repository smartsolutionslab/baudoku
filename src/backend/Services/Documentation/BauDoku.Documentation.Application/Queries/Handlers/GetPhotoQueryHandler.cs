using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetPhotoQueryHandler(IPhotoReadRepository photos) : IQueryHandler<GetPhotoQuery, PhotoDto>
{
    public async Task<PhotoDto> Handle(GetPhotoQuery query, CancellationToken cancellationToken = default)
    {
        var photoId = query.PhotoId;

        return await photos.GetByIdAsync(photoId, cancellationToken);
    }
}
