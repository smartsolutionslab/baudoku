import { Outlet, Link, useLocation } from '@tanstack/react-router';
import { useState } from 'react';
import { LayoutIcon, FolderIcon, MenuIcon } from '../components/icons';
import { VersionInfo } from '../components/layout/VersionInfo';
import { UserSection } from '../components/layout/UserSection';

const navItems = [
  { to: '/', label: 'Dashboard', icon: LayoutIcon },
  { to: '/projects', label: 'Projekte', icon: FolderIcon },
] as const;

export function RootLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const location = useLocation();

  // Don't render shell on login page
  if (location.pathname === '/login') {
    return <Outlet />;
  }

  return (
    <div className='flex h-screen overflow-hidden bg-gray-50'>
      {/* Mobile overlay */}
      {sidebarOpen && (
        <div
          className='fixed inset-0 z-30 bg-black/50 lg:hidden'
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed inset-y-0 left-0 z-40 flex w-64 flex-col bg-white border-r border-gray-200 transition-transform duration-200 lg:static lg:translate-x-0 ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        {/* Logo */}
        <div className='flex h-16 items-center gap-2 border-b border-gray-200 px-6'>
          <div className='flex h-8 w-8 items-center justify-center rounded-lg bg-blue-600 text-sm font-bold text-white'>
            B
          </div>
          <span className='text-lg font-semibold text-gray-900'>BauDoku</span>
        </div>

        {/* Navigation */}
        <nav className='flex-1 space-y-1 p-4'>
          {navItems.map((item) => {
            const isActive =
              item.to === '/'
                ? location.pathname === '/'
                : location.pathname.startsWith(item.to);
            return (
              <Link
                key={item.to}
                to={item.to}
                onClick={() => setSidebarOpen(false)}
                className={`flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
                  isActive
                    ? 'bg-blue-50 text-blue-700'
                    : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                }`}
              >
                <item.icon />
                {item.label}
              </Link>
            );
          })}
        </nav>

        {/* Version info */}
        <VersionInfo />

        {/* User section */}
        <UserSection />
      </aside>

      {/* Main content */}
      <div className='flex flex-1 flex-col overflow-hidden'>
        {/* Topbar */}
        <header className='flex h-16 items-center gap-4 border-b border-gray-200 bg-white px-4 lg:px-6'>
          <button
            className='rounded-lg p-2 text-gray-500 hover:bg-gray-100 lg:hidden'
            onClick={() => setSidebarOpen(true)}
          >
            <MenuIcon />
          </button>
          <div className='flex-1' />
        </header>

        {/* Page content */}
        <main className='flex-1 overflow-y-auto p-4 lg:p-6'>
          <Outlet />
        </main>
      </div>
    </div>
  );
}
