using BauDoku.ResourceService.Services;
using BauDoku.ServiceDefaults;
using k8s;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGrpc();

builder.Services.AddSingleton<IKubernetes>(_ =>
{
    var config = KubernetesClientConfiguration.IsInCluster()
        ? KubernetesClientConfiguration.InClusterConfig()
        : KubernetesClientConfiguration.BuildConfigFromConfigFile();
    return new Kubernetes(config);
});

builder.Services.AddSingleton<PodWatcher>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PodWatcher>());
builder.Services.AddSingleton<PodLogStreamer>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapGrpcService<KubernetesDashboardService>();
app.MapGet("/", () => "BauDoku K8s Resource Service for Aspire Dashboard");

app.Run();
