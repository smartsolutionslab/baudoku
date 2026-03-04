import { useAuth } from '@/auth';
import { LogoutIcon } from '../icons';

export function UserSection() {
  const { user, logout, isAuthenticated } = useAuth();

  if (!isAuthenticated) return null;

  return (
    <div className="border-t border-gray-200 p-4">
      <div className="flex items-center gap-3">
        <div className="flex h-8 w-8 items-center justify-center rounded-full bg-gray-200 text-sm font-medium text-gray-600">
          {user?.profile?.name?.[0]?.toUpperCase() ?? '?'}
        </div>
        <div className="flex-1 min-w-0">
          <p className="truncate text-sm font-medium text-gray-900">
            {user?.profile?.name ?? 'Benutzer'}
          </p>
        </div>
        <button
          onClick={logout}
          className="rounded p-1 text-gray-400 hover:text-gray-600"
          title="Abmelden"
        >
          <LogoutIcon />
        </button>
      </div>
    </div>
  );
}
