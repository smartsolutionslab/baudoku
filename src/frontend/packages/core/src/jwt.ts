export function parseJwtPayload(token: string): Record<string, unknown> {
  const parts = token.split(".");
  if (parts.length !== 3) throw new Error("Invalid JWT format");
  const payload = parts[1]!;
  const decoded = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
  return JSON.parse(decoded);
}

export function isTokenExpired(token: string, bufferSeconds = 30): boolean {
  try {
    const payload = parseJwtPayload(token);
    const exp = payload.exp as number | undefined;
    if (!exp) return true;
    return Date.now() / 1000 > exp - bufferSeconds;
  } catch {
    return true;
  }
}
