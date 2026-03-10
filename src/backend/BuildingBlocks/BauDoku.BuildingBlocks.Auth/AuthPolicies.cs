namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Auth;

public static class AuthRoles
{
    public const string Inspector = "inspector";
    public const string User = "user";
    public const string Admin = "admin";
}

public static class AuthPolicies
{
    public const string RequireInspector = "RequireInspector";
    public const string RequireUser = "RequireUser";
    public const string RequireAdmin = "RequireAdmin";
}
