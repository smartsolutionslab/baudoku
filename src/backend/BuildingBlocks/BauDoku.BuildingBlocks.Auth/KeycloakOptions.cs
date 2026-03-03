namespace BauDoku.BuildingBlocks.Auth;

public sealed class KeycloakOptions
{
    public const string SectionName = "Authentication:Keycloak";

    public string Authority { get; set; } = "";
    public string ClientId { get; set; } = "baudoku-app";
    public string Audience { get; set; } = "baudoku-api";
    public string[]? AdditionalIssuers { get; set; }
    public string[]? Audiences { get; set; }
}
