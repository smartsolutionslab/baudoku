import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
  type ReactNode,
} from "react";
import { UserManager, type User } from "oidc-client-ts";
import { oidcConfig } from "./oidcConfig";
import { setAuthToken, onUnauthorized } from "@baudoku/shared-api";

interface AuthContextValue {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: () => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export const userManager = new UserManager(oidcConfig);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for existing session
    userManager
      .getUser()
      .then((existingUser) => {
        if (existingUser && !existingUser.expired) {
          setUser(existingUser);
          setAuthToken(existingUser.access_token);
        }
      })
      .finally(() => setIsLoading(false));

    // Handle token updates
    const onUserLoaded = (loadedUser: User) => {
      setUser(loadedUser);
      setAuthToken(loadedUser.access_token);
    };

    const onUserUnloaded = () => {
      setUser(null);
      setAuthToken(null);
    };

    userManager.events.addUserLoaded(onUserLoaded);
    userManager.events.addUserUnloaded(onUserUnloaded);
    userManager.events.addAccessTokenExpired(onUserUnloaded);

    // Wire API 401 handler to trigger re-login
    const unsubscribe = onUnauthorized(() => {
      userManager.signinRedirect();
    });

    return () => {
      userManager.events.removeUserLoaded(onUserLoaded);
      userManager.events.removeUserUnloaded(onUserUnloaded);
      userManager.events.removeAccessTokenExpired(onUserUnloaded);
      unsubscribe();
    };
  }, []);

  const login = useCallback(async () => {
    await userManager.signinRedirect();
  }, []);

  const logout = useCallback(async () => {
    await userManager.signoutRedirect();
  }, []);

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user && !user.expired,
        isLoading,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
