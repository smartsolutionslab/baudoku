namespace BauDoku.Sync.Api.Endpoints;

public sealed record ResolveConflictRequest(string Strategy, string? MergedPayload);
