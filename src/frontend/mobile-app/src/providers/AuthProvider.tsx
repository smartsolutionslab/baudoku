import React, { useEffect, useRef, useCallback } from "react";
import { useAuthStore } from "../store";
import { loadTokens, saveTokens, refreshAccessToken, parseUserFromToken, performLogout } from "../auth";
import { setAuthToken, setBaseUrl, onUnauthorized } from "@baudoku/core";
import { API_BASE_URL } from "../config/environment";

function getTokenExpiresIn(token: string): number {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) return 0;
    const payload = JSON.parse(atob(parts[1]));
    const exp = payload.exp as number;
    if (!exp) return 0;
    return exp * 1000 - Date.now();
  } catch {
    return 0;
  }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const refreshTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const { setTokens, setUser } = useAuthStore();

  useEffect(() => {
    setBaseUrl(API_BASE_URL);
  }, []);

  const handleLogout = useCallback(async () => {
    await performLogout();
    if (refreshTimerRef.current) {
      clearTimeout(refreshTimerRef.current);
      refreshTimerRef.current = null;
    }
  }, []);

  const scheduleRefresh = useCallback(
    (accessToken: string, currentRefreshToken: string, currentIdToken: string) => {
      if (refreshTimerRef.current) {
        clearTimeout(refreshTimerRef.current);
      }

      const expiresIn = getTokenExpiresIn(accessToken);
      const refreshIn = Math.max(expiresIn - 30_000, 5_000);

      refreshTimerRef.current = setTimeout(async () => {
        try {
          const tokens = await refreshAccessToken(currentRefreshToken);
          const newIdToken = tokens.idToken || currentIdToken;
          await saveTokens(tokens.accessToken, tokens.refreshToken, newIdToken);
          setTokens(tokens.accessToken, tokens.refreshToken, newIdToken);
          setAuthToken(tokens.accessToken);

          if (tokens.idToken) {
            const user = parseUserFromToken(tokens.idToken);
            setUser(user);
          }

          scheduleRefresh(tokens.accessToken, tokens.refreshToken, newIdToken);
        } catch {
          await handleLogout();
        }
      }, refreshIn);
    },
    [setTokens, setUser, handleLogout]
  );

  useEffect(() => {
    let mounted = true;

    async function initAuth() {
      const { accessToken, refreshToken, idToken } = await loadTokens();

      if (!mounted || !accessToken || !refreshToken) return;

      const expiresIn = getTokenExpiresIn(accessToken);

      if (expiresIn > 30_000) {
        setTokens(accessToken, refreshToken, idToken ?? "");
        setAuthToken(accessToken);
        try {
          const tokenToParse = idToken || accessToken;
          const user = parseUserFromToken(tokenToParse);
          setUser(user);
        } catch {
          // Token parsing failed — user info will be missing
        }
        scheduleRefresh(accessToken, refreshToken, idToken ?? "");
      } else {
        try {
          const tokens = await refreshAccessToken(refreshToken);
          if (!mounted) return;
          const newIdToken = tokens.idToken || idToken || "";
          await saveTokens(tokens.accessToken, tokens.refreshToken, newIdToken);
          setTokens(tokens.accessToken, tokens.refreshToken, newIdToken);
          setAuthToken(tokens.accessToken);

          if (tokens.idToken) {
            const user = parseUserFromToken(tokens.idToken);
            setUser(user);
          }

          scheduleRefresh(tokens.accessToken, tokens.refreshToken, newIdToken);
        } catch {
          if (mounted) await handleLogout();
        }
      }
    }

    void initAuth();

    return () => {
      mounted = false;
      if (refreshTimerRef.current) {
        clearTimeout(refreshTimerRef.current);
      }
    };
  }, [setTokens, setUser, scheduleRefresh, handleLogout]);

  useEffect(() => {
    const unsubscribe = onUnauthorized(() => {
      void handleLogout();
    });
    return unsubscribe;
  }, [handleLogout]);

  return <>{children}</>;
}
