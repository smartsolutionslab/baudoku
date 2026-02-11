import Constants from "expo-constants";

const extra = Constants.expoConfig?.extra ?? {};

export const API_BASE_URL: string =
  (extra.apiBaseUrl as string) || "http://localhost:5000";

export const KEYCLOAK_URL: string =
  (extra.keycloakUrl as string) || "http://localhost:8080";

export const KEYCLOAK_REALM: string =
  (extra.keycloakRealm as string) || "baudoku";

export const KEYCLOAK_CLIENT_ID: string =
  (extra.keycloakClientId as string) || "baudoku-mobile";
