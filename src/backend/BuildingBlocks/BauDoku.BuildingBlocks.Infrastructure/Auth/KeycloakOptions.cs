namespace BauDoku.BuildingBlocks.Infrastructure.Auth;

public sealed class KeycloakOptions
{
    public string Authority { get; set; } = "";
    public string ClientId { get; set; } = "baudoku-app";
    public string Audience { get; set; } = "baudoku-api";
    public string[]? AdditionalIssuers { get; set; }
    public string[]? Audiences { get; set; }
}
