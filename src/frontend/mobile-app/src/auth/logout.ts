import { apiPost } from "@baudoku/core";
import { setAuthToken } from "@baudoku/core";
import { useAuthStore } from "../store";
import { logoutFromKeycloak } from "./keycloak";
import { clearTokens } from "./tokenStorage";

export async function performLogout(): Promise<void> {
  const { refreshToken, idToken, clearAuth } = useAuthStore.getState();

  // 1. Revoke refresh token via backend (best-effort)
  if (refreshToken) {
    try {
      await apiPost<void>("/api/auth/logout", { refreshToken });
    } catch {
      // Backend revocation failed — continue with local cleanup
    }
  }

  // 2. End Keycloak session (best-effort)
  if (idToken) {
    try {
      await logoutFromKeycloak(idToken);
    } catch {
      // Keycloak session end failed — continue with local cleanup
    }
  }

  // 3. Clear local state
  clearAuth();
  setAuthToken(null);
  await clearTokens();
}
