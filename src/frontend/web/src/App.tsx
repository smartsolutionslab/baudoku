import { RouterProvider } from '@tanstack/react-router';
import { useAuth } from './auth';
import { AppProviders } from './providers';
import { router } from './router';
import { LoadingSpinner } from './components/common';

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
        <LoadingSpinner />
      </div>
    );
  }

  return <RouterProvider router={router} context={{ auth: { isAuthenticated } }} />;
}
