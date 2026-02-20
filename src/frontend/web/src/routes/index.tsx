import { useMemo } from "react";
import { Link } from "@tanstack/react-router";
import { useProjects } from "@/hooks/useProjects";
import { PlusIcon } from "@/components/icons";

export function DashboardPage() {
  const { data } = useProjects();

  const projects = useMemo(
    () => data?.pages.flatMap((p) => p.items) ?? [],
    [data]
  );
  const totalCount = data?.pages[0]?.totalCount ?? 0;
  const activeCount = projects.filter((p) => p.status === "active").length;
  const completedCount = projects.filter((p) => p.status === "completed").length;

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
      <p className="mt-1 text-sm text-gray-500">
        Willkommen bei BauDoku
      </p>

      {/* Stats */}
      <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <DashboardCard
          title="Aktive Projekte"
          value={activeCount}
          color="blue"
        />
        <DashboardCard
          title="Abgeschlossen"
          value={completedCount}
          color="green"
        />
        <DashboardCard
          title="Gesamt"
          value={totalCount}
          color="gray"
        />
      </div>

      {/* Quick actions */}
      <div className="mt-8">
        <h2 className="text-lg font-semibold text-gray-900">Schnellaktionen</h2>
        <div className="mt-3 flex flex-wrap gap-3">
          <Link
            to="/projects/new"
            className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-blue-700 transition-colors"
          >
            <PlusIcon />
            Neues Projekt
          </Link>
          <Link
            to="/projects"
            className="inline-flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50 transition-colors"
          >
            Alle Projekte anzeigen
          </Link>
        </div>
      </div>

      {/* Recent projects */}
      {projects.length > 0 && (
        <div className="mt-8">
          <h2 className="text-lg font-semibold text-gray-900">
            Letzte Projekte
          </h2>
          <div className="mt-3 grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            {projects.slice(0, 6).map((project) => (
              <Link
                key={project.id}
                to="/projects/$projectId"
                params={{ projectId: project.id }}
                className="rounded-lg border border-gray-200 bg-white p-4 hover:border-blue-300 hover:shadow-sm transition-all"
              >
                <h3 className="font-medium text-gray-900">{project.name}</h3>
                {project.city && (
                  <p className="mt-1 text-sm text-gray-500">{project.city}</p>
                )}
                <StatusBadge status={project.status} className="mt-2" />
              </Link>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

function DashboardCard({
  title,
  value,
  color,
}: {
  title: string;
  value: number;
  color: "blue" | "green" | "gray";
}) {
  const colorClasses = {
    blue: "bg-blue-50 text-blue-700",
    green: "bg-green-50 text-green-700",
    gray: "bg-gray-50 text-gray-700",
  };

  return (
    <div className="rounded-xl border border-gray-200 bg-white p-6">
      <p className="text-sm font-medium text-gray-500">{title}</p>
      <p className={`mt-2 text-3xl font-bold ${colorClasses[color]}`}>
        {value}
      </p>
    </div>
  );
}

function StatusBadge({ status, className = "" }: { status: string; className?: string }) {
  const styles: Record<string, string> = {
    active: "bg-green-100 text-green-700",
    completed: "bg-blue-100 text-blue-700",
    archived: "bg-gray-100 text-gray-600",
  };
  const labels: Record<string, string> = {
    active: "Aktiv",
    completed: "Abgeschlossen",
    archived: "Archiviert",
  };

  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${styles[status] ?? styles.active} ${className}`}
    >
      {labels[status] ?? status}
    </span>
  );
}
