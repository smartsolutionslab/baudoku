import type { AuthUser } from './auth';

export function parseUserFromToken(idToken: string): AuthUser {
  const payload = parseJwtPayload(idToken);

  if (typeof payload.exp === 'number' && payload.exp * 1000 < Date.now()) {
    throw new Error('Token ist abgelaufen');
  }

  return {
    id: (payload.sub as string) ?? '',
    email: (payload.email as string) ?? '',
    name: (payload.name as string) ?? [payload.preferred_username].filter(Boolean).join(' ') ?? '',
    roles: (payload.realm_access as { roles?: string[] })?.roles ?? [],
  };
}

export function parseJwtPayload(token: string): Record<string, unknown> {
  const parts = token.split('.');
  if (parts.length !== 3) throw new Error('Invalid JWT format');
  const payload = parts[1]!;
  const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
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
