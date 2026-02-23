import type { UserManagerSettings } from "oidc-client-ts";

const runtimeConfig = (window as any).__BAUDOKU_CONFIG__ ?? {};
const keycloakUrl = runtimeConfig.KEYCLOAK_URL || import.meta.env.VITE_KEYCLOAK_URL || "http://localhost:8080";
const realm = runtimeConfig.KEYCLOAK_REALM || import.meta.env.VITE_KEYCLOAK_REALM || "baudoku";
const clientId = runtimeConfig.KEYCLOAK_CLIENT_ID || import.meta.env.VITE_KEYCLOAK_CLIENT_ID || "baudoku-web";

export const oidcConfig: UserManagerSettings = {
  authority: `${keycloakUrl}/realms/${realm}`,
  client_id: clientId,
  redirect_uri: `${window.location.origin}/login`,
  post_logout_redirect_uri: window.location.origin,
  response_type: "code",
  scope: "openid profile email",
  automaticSilentRenew: true,
  silent_redirect_uri: `${window.location.origin}/login`,
};
