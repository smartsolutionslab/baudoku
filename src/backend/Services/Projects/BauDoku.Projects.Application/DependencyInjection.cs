using BauDoku.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Projects.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsApplication(this IServiceCollection services)
        => services.AddApplication(typeof(DependencyInjection).Assembly);
}
