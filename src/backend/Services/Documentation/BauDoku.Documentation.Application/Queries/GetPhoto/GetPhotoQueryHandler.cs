using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Queries.GetPhoto;

public sealed class GetPhotoQueryHandler : IQueryHandler<GetPhotoQuery, PhotoDto?>
{
    private readonly IPhotoReadRepository photoReadRepository;

    public GetPhotoQueryHandler(IPhotoReadRepository photoReadRepository)
    {
        this.photoReadRepository = photoReadRepository;
    }

    public async Task<PhotoDto?> Handle(GetPhotoQuery query, CancellationToken cancellationToken)
    {
        return await photoReadRepository.GetByIdAsync(query.PhotoId, cancellationToken);
    }
}
