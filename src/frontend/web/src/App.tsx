import { RouterProvider } from '@tanstack/react-router';
import { useAuth } from './auth';
import { AppProviders } from './providers';
import { router } from './router';

export function App() {
  return (
    <AppProviders>
      <AuthenticatedRouter />
    </AppProviders>
  );
}

function AuthenticatedRouter() {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center bg-gray-50">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-gray-200 border-t-blue-600" />
      </div>
    );
  }

  return <RouterProvider router={router} context={{ auth: { isAuthenticated } }} />;
}
