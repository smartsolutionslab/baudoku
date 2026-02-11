using BauDoku.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:8081",
                "http://localhost:19006",
                "exp://localhost:8081",
                "http://10.0.0.6:8081",
                "http://10.0.0.6:5000",
                "exp://10.0.0.6:8081")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Authorization");
    });
});

var app = builder.Build();

app.UseCors();
app.MapReverseProxy();
app.MapDefaultEndpoints();

app.Run();
