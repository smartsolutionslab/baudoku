using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetPhoto;

public sealed class GetPhotoQueryHandler(IPhotoReadRepository photos) : IQueryHandler<GetPhotoQuery, PhotoDto?>
{
    public async Task<PhotoDto?> Handle(GetPhotoQuery query, CancellationToken cancellationToken)
    {
        return await photos.GetByIdAsync(query.PhotoId, cancellationToken);
    }
}
