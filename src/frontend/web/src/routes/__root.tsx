import { Outlet, Link, useLocation } from "@tanstack/react-router";
import { useState, useEffect } from "react";
import { useAuth } from "../auth/AuthProvider";

const navItems = [
  { to: "/", label: "Dashboard", icon: LayoutIcon },
  { to: "/projects", label: "Projekte", icon: FolderIcon },
] as const;

export function RootLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const location = useLocation();

  // Don't render shell on login page
  if (location.pathname === "/login") {
    return <Outlet />;
  }

  return (
    <div className="flex h-screen overflow-hidden bg-gray-50">
      {/* Mobile overlay */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 z-30 bg-black/50 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed inset-y-0 left-0 z-40 flex w-64 flex-col bg-white border-r border-gray-200 transition-transform duration-200 lg:static lg:translate-x-0 ${
          sidebarOpen ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        {/* Logo */}
        <div className="flex h-16 items-center gap-2 border-b border-gray-200 px-6">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-blue-600 text-sm font-bold text-white">
            B
          </div>
          <span className="text-lg font-semibold text-gray-900">BauDoku</span>
        </div>

        {/* Navigation */}
        <nav className="flex-1 space-y-1 p-4">
          {navItems.map((item) => {
            const isActive =
              item.to === "/"
                ? location.pathname === "/"
                : location.pathname.startsWith(item.to);
            return (
              <Link
                key={item.to}
                to={item.to}
                onClick={() => setSidebarOpen(false)}
                className={`flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
                  isActive
                    ? "bg-blue-50 text-blue-700"
                    : "text-gray-600 hover:bg-gray-100 hover:text-gray-900"
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
      <div className="flex flex-1 flex-col overflow-hidden">
        {/* Topbar */}
        <header className="flex h-16 items-center gap-4 border-b border-gray-200 bg-white px-4 lg:px-6">
          <button
            className="rounded-lg p-2 text-gray-500 hover:bg-gray-100 lg:hidden"
            onClick={() => setSidebarOpen(true)}
          >
            <MenuIcon />
          </button>
          <div className="flex-1" />
        </header>

        {/* Page content */}
        <main className="flex-1 overflow-y-auto p-4 lg:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

type ServiceInfo = {
  service: string;
  version: string;
  status?: string;
};

type SystemInfo = {
  gateway: ServiceInfo;
  services: ServiceInfo[];
};

function VersionInfo() {
  const [expanded, setExpanded] = useState(false);
  const [systemInfo, setSystemInfo] = useState<SystemInfo | null>(null);

  useEffect(() => {
    if (!expanded || systemInfo) return;
    fetch("/api/system/info")
      .then((res) => (res.ok ? res.json() : null))
      .then((data) => setSystemInfo(data))
      .catch(() => {});
  }, [expanded, systemInfo]);

  return (
    <div className="border-t border-gray-200 px-4 py-2">
      <button
        onClick={() => setExpanded(!expanded)}
        className="text-xs text-gray-400 hover:text-gray-600 transition-colors"
      >
        v{__APP_VERSION__}
      </button>
      {expanded && (
        <div className="mt-2 space-y-1 text-xs text-gray-500">
          <p className="font-medium text-gray-600">Services</p>
          {systemInfo ? (
            <>
              <ServiceRow
                label="Gateway"
                version={systemInfo.gateway.version}
                status="ok"
              />
              {systemInfo.services.map((s) => (
                <ServiceRow
                  key={s.service}
                  label={s.service.replace("BauDoku.", "").replace(".Api", "")}
                  version={s.version}
                  status={s.status ?? "ok"}
                />
              ))}
            </>
          ) : (
            <p className="text-gray-400">Laden...</p>
          )}
        </div>
      )}
    </div>
  );
}

function ServiceRow({
  label,
  version,
  status,
}: {
  label: string;
  version: string;
  status: string;
}) {
  return (
    <div className="flex items-center justify-between">
      <span>{label}</span>
      <span className="flex items-center gap-1">
        <span
          className={`inline-block h-1.5 w-1.5 rounded-full ${status === "ok" ? "bg-green-400" : "bg-red-400"}`}
        />
        {version}
      </span>
    </div>
  );
}

function UserSection() {
  const { user, logout, isAuthenticated } = useAuth();

  if (!isAuthenticated) return null;

  return (
    <div className="border-t border-gray-200 p-4">
      <div className="flex items-center gap-3">
        <div className="flex h-8 w-8 items-center justify-center rounded-full bg-gray-200 text-sm font-medium text-gray-600">
          {user?.profile?.name?.[0]?.toUpperCase() ?? "?"}
        </div>
        <div className="flex-1 min-w-0">
          <p className="truncate text-sm font-medium text-gray-900">
            {user?.profile?.name ?? "Benutzer"}
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

// ─── Icons (inline SVG) ─────────────────────────────────────────

function LayoutIcon() {
  return (
    <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6A2.25 2.25 0 0 1 6 3.75h2.25A2.25 2.25 0 0 1 10.5 6v2.25a2.25 2.25 0 0 1-2.25 2.25H6a2.25 2.25 0 0 1-2.25-2.25V6ZM3.75 15.75A2.25 2.25 0 0 1 6 13.5h2.25a2.25 2.25 0 0 1 2.25 2.25V18a2.25 2.25 0 0 1-2.25 2.25H6A2.25 2.25 0 0 1 3.75 18v-2.25ZM13.5 6a2.25 2.25 0 0 1 2.25-2.25H18A2.25 2.25 0 0 1 20.25 6v2.25A2.25 2.25 0 0 1 18 10.5h-2.25a2.25 2.25 0 0 1-2.25-2.25V6ZM13.5 15.75a2.25 2.25 0 0 1 2.25-2.25H18a2.25 2.25 0 0 1 2.25 2.25V18A2.25 2.25 0 0 1 18 20.25h-2.25a2.25 2.25 0 0 1-2.25-2.25v-2.25Z" />
    </svg>
  );
}

function FolderIcon() {
  return (
    <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 12.75V12A2.25 2.25 0 0 1 4.5 9.75h15A2.25 2.25 0 0 1 21.75 12v.75m-8.69-6.44-2.12-2.12a1.5 1.5 0 0 0-1.061-.44H4.5A2.25 2.25 0 0 0 2.25 6v12a2.25 2.25 0 0 0 2.25 2.25h15A2.25 2.25 0 0 0 21.75 18V9a2.25 2.25 0 0 0-2.25-2.25h-5.379a1.5 1.5 0 0 1-1.06-.44Z" />
    </svg>
  );
}

function MenuIcon() {
  return (
    <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
    </svg>
  );
}

function LogoutIcon() {
  return (
    <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0 0 13.5 3h-6a2.25 2.25 0 0 0-2.25 2.25v13.5A2.25 2.25 0 0 0 7.5 21h6a2.25 2.25 0 0 0 2.25-2.25V15m3 0 3-3m0 0-3-3m3 3H9" />
    </svg>
  );
}
