import type { UserManagerSettings } from "oidc-client-ts";

const keycloakUrl = import.meta.env.VITE_KEYCLOAK_URL || "http://localhost:8080";
const realm = import.meta.env.VITE_KEYCLOAK_REALM || "baudoku";
const clientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID || "baudoku-web";

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
