using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Queries;

public sealed record GetMeasurementsQuery(InstallationIdentifier InstallationId) : IQuery<IReadOnlyList<MeasurementDto>>;
