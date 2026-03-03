using SmartSolutionsLab.BauDoku.ResourceService;
using SmartSolutionsLab.BauDoku.ResourceService.Services;
using k8s;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.Configure<KubernetesOptions>(options =>
{
    builder.Configuration.GetSection(KubernetesOptions.SectionName).Bind(options);

    var envNamespace = Environment.GetEnvironmentVariable("KUBERNETES_NAMESPACE");
    if (!string.IsNullOrEmpty(envNamespace))
    {
        options.Namespace = envNamespace;
    }

    var envAppName = Environment.GetEnvironmentVariable("APPLICATION_NAME");
    if (!string.IsNullOrEmpty(envAppName))
    {
        options.ApplicationName = envAppName;
    }
});

builder.Services.AddSingleton<IKubernetes>(_ =>
{
    var config = KubernetesClientConfiguration.IsInCluster()
        ? KubernetesClientConfiguration.InClusterConfig()
        : KubernetesClientConfiguration.BuildConfigFromConfigFile();
    return new Kubernetes(config);
});

builder.Services.AddSingleton<PodWatcher>()
                .AddHostedService(sp => sp.GetRequiredService<PodWatcher>())
                .AddSingleton<PodLogStreamer>();

var app = builder.Build();

app.MapGrpcService<KubernetesDashboardService>();

app.Run();
