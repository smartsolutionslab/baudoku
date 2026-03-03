using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Persistence;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Persistence;
using SmartSolutionsLab.BauDoku.Projects.Application.Contracts;
using SmartSolutionsLab.BauDoku.Projects.ReadModel;
using SmartSolutionsLab.BauDoku.Projects.Domain;
using SmartSolutionsLab.BauDoku.Projects.Infrastructure.Persistence;
using SmartSolutionsLab.BauDoku.Projects.Infrastructure.Persistence.Repositories;
using SmartSolutionsLab.BauDoku.Projects.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.BauDoku.Projects.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ProjectsDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<ProjectsReadDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork<ProjectsDbContext>>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
        services.AddScoped<IProjectCountProvider, ProjectCountProvider>();
        services.AddHostedService<ActiveProjectCountService>();

        return services;
    }
}
