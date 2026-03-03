import * as AuthSession from 'expo-auth-session';
import { KEYCLOAK_URL, KEYCLOAK_REALM, KEYCLOAK_CLIENT_ID } from '../config/environment';
import { parseJwtPayload } from '@baudoku/core';
import type { AuthUser } from '../store';

const realmUrl = `${KEYCLOAK_URL}/realms/${KEYCLOAK_REALM}`;

const discovery: AuthSession.DiscoveryDocument = {
  authorizationEndpoint: `${realmUrl}/protocol/openid-connect/auth`,
  tokenEndpoint: `${realmUrl}/protocol/openid-connect/token`,
  endSessionEndpoint: `${realmUrl}/protocol/openid-connect/logout`,
};

const redirectUri = AuthSession.makeRedirectUri({
  scheme: 'baudoku',
  path: 'auth/callback',
});

export type AuthTokens = {
  accessToken: string;
  refreshToken: string;
  idToken: string;
};

export async function loginWithKeycloak(): Promise<AuthTokens> {
  const request = new AuthSession.AuthRequest({
    clientId: KEYCLOAK_CLIENT_ID,
    redirectUri,
    scopes: ['openid', 'profile', 'email'],
    usePKCE: true,
    responseType: AuthSession.ResponseType.Code,
  });

  const result = await request.promptAsync(discovery);

  if (result.type !== 'success' || !result.params.code) {
    const message = result.type === 'cancel' ? 'Anmeldung abgebrochen' : `Anmeldung fehlgeschlagen: ${result.type}`;
    throw new Error(message);
  }

  const tokenResponse = await AuthSession.exchangeCodeAsync(
    {
      clientId: KEYCLOAK_CLIENT_ID,
      code: result.params.code,
      redirectUri,
      extraParams: { code_verifier: request.codeVerifier! },
    },
    discovery
  );

  if (!tokenResponse.accessToken || !tokenResponse.refreshToken || !tokenResponse.idToken) {
    throw new Error('Unvollständige Token-Antwort vom Server');
  }

  return {
    accessToken: tokenResponse.accessToken,
    refreshToken: tokenResponse.refreshToken,
    idToken: tokenResponse.idToken,
  };
}

export async function refreshAccessToken(refreshToken: string): Promise<AuthTokens> {
  const tokenResponse = await AuthSession.refreshAsync(
    {
      clientId: KEYCLOAK_CLIENT_ID,
      refreshToken,
    },
    discovery
  );

  if (!tokenResponse.accessToken || !tokenResponse.refreshToken) {
    throw new Error('Token-Aktualisierung fehlgeschlagen');
  }

  return {
    accessToken: tokenResponse.accessToken,
    refreshToken: tokenResponse.refreshToken ?? refreshToken,
    idToken: tokenResponse.idToken ?? '',
  };
}

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

export async function logoutFromKeycloak(idToken: string): Promise<void> {
  if (!discovery.endSessionEndpoint) return;

  try {
    await fetch(`${discovery.endSessionEndpoint}?id_token_hint=${idToken}&post_logout_redirect_uri=${encodeURIComponent(redirectUri)}`, { method: 'GET' });
  } catch {
    // Logout-Fehler ignorieren — Tokens werden lokal gelöscht
  }
}
