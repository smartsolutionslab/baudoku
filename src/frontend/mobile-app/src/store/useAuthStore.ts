import { create } from 'zustand';
import type { AuthUser } from '@baudoku/core';

export type { AuthUser } from '@baudoku/core';

type AuthState = {
  isAuthenticated: boolean;
  accessToken: string | null;
  refreshToken: string | null;
  idToken: string | null;
  user: AuthUser | null;
  setTokens: (accessToken: string, refreshToken: string, idToken: string) => void;
  setUser: (user: AuthUser) => void;
  clearAuth: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  isAuthenticated: false,
  accessToken: null,
  refreshToken: null,
  idToken: null,
  user: null,

  setTokens: (accessToken, refreshToken, idToken) =>
    set({ accessToken, refreshToken, idToken, isAuthenticated: true }),

  setUser: (user) => set({ user }),

  clearAuth: () =>
    set({
      isAuthenticated: false,
      accessToken: null,
      refreshToken: null,
      idToken: null,
      user: null,
    }),
}));
