import { useEffect } from "react";
import { useNavigate } from "@tanstack/react-router";
import { UserManager } from "oidc-client-ts";
import { oidcConfig } from "../auth/oidcConfig";
import { useAuth } from "../auth/AuthProvider";

const userManager = new UserManager(oidcConfig);

export function LoginPage() {
  const navigate = useNavigate();
  const { isAuthenticated, login } = useAuth();

  useEffect(() => {
    // Handle OIDC callback
    if (window.location.search.includes("code=")) {
      userManager
        .signinRedirectCallback()
        .then(() => navigate({ to: "/" }))
        .catch(() => navigate({ to: "/" }));
      return;
    }

    // Already logged in → go home
    if (isAuthenticated) {
      navigate({ to: "/" });
      return;
    }

    // Not logged in → redirect to Keycloak
    login();
  }, [isAuthenticated, navigate, login]);

  return (
    <div className="flex h-screen items-center justify-center bg-gray-50">
      <div className="text-center">
        <div className="mb-4 flex justify-center">
          <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-blue-600 text-2xl font-bold text-white">
            B
          </div>
        </div>
        <h1 className="text-xl font-semibold text-gray-900">BauDoku</h1>
        <p className="mt-2 text-sm text-gray-500">Anmeldung wird verarbeitet...</p>
        <div className="mt-4">
          <div className="mx-auto h-8 w-8 animate-spin rounded-full border-4 border-gray-200 border-t-blue-600" />
        </div>
      </div>
    </div>
  );
}
