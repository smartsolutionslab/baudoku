using SmartSolutionsLab.BauDoku.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.BauDoku.Documentation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentationApplication(this IServiceCollection services)
        => services.AddApplication(typeof(DependencyInjection).Assembly);
}
