using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Queries.GetPhoto;

public sealed class GetPhotoQueryHandler : IQueryHandler<GetPhotoQuery, PhotoDto?>
{
    private readonly IPhotoReadRepository photos;

    public GetPhotoQueryHandler(IPhotoReadRepository photos)
    {
        this.photos = photos;
    }

    public async Task<PhotoDto?> Handle(GetPhotoQuery query, CancellationToken cancellationToken)
    {
        return await photos.GetByIdAsync(query.PhotoId, cancellationToken);
    }
}
