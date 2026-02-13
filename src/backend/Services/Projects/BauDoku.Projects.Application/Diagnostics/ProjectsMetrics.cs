using System.Diagnostics.Metrics;

namespace BauDoku.Projects.Application.Diagnostics;

public static class ProjectsMetrics
{
    private static readonly Meter Meter = new("BauDoku.Projects");

    public static readonly Counter<long> ProjectsCreated =
        Meter.CreateCounter<long>("baudoku.projects.created", description: "Number of projects created");

    public static readonly Counter<long> ZonesAdded =
        Meter.CreateCounter<long>("baudoku.projects.zones_added", description: "Number of zones added to projects");

    public static readonly Histogram<int> ZonesPerProject =
        Meter.CreateHistogram<int>("baudoku.projects.zones_per_project", description: "Number of zones per project after adding a zone");

    private static int _activeProjectCount;

    public static readonly ObservableGauge<int> ActiveProjectCount =
        Meter.CreateObservableGauge("baudoku.projects.active_count", () => _activeProjectCount, description: "Number of active projects");

    public static void SetActiveProjectCount(int count) => _activeProjectCount = count;
}
