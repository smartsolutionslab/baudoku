using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.ReadModel;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries;

public sealed record GetPhotoQuery(PhotoIdentifier PhotoId) : IQuery<PhotoDto>;
