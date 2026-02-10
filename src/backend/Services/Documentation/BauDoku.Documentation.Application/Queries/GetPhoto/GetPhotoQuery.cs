using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetPhoto;

public sealed record GetPhotoQuery(Guid PhotoId) : IQuery<PhotoDto?>;
