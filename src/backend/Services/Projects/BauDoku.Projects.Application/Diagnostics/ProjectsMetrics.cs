using System.Diagnostics.Metrics;

namespace BauDoku.Projects.Application.Diagnostics;

public static class ProjectsMetrics
{
    private static readonly Meter Meter = new("BauDoku.Projects");

    public static readonly Counter<long> ProjectsCreated =
        Meter.CreateCounter<long>("baudoku.projects.created", description: "Number of projects created");

    public static readonly Counter<long> ZonesAdded =
        Meter.CreateCounter<long>("baudoku.projects.zones_added", description: "Number of zones added to projects");
}
