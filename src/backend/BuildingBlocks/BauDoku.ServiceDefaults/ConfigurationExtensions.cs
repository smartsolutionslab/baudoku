using Microsoft.Extensions.Configuration;

namespace BauDoku.ServiceDefaults;

public static class ConfigurationExtensions
{
    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
        => configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Connection string '{name}' not found.");
}
