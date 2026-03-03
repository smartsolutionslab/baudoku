using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetPhotoQueryHandler(IPhotoReadRepository photos) : IQueryHandler<GetPhotoQuery, PhotoDto>
{
    public async Task<PhotoDto> Handle(GetPhotoQuery query, CancellationToken cancellationToken = default)
    {
        var photoId = query.PhotoId;

        return await photos.With(photoId, cancellationToken);
    }
}
