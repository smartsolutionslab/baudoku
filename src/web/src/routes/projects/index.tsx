import { useState, useMemo } from "react";
import { Link } from "@tanstack/react-router";
import { useProjects, useDeleteProject } from "../../hooks/useProjects";
import { StatusBadge } from "../../components/common/StatusBadge";
import { SearchBar } from "../../components/common/SearchBar";
import { FilterChips } from "../../components/common/FilterChips";
import { EmptyState } from "../../components/common/EmptyState";
import { ConfirmDialog } from "../../components/common/ConfirmDialog";
import { PROJECT_STATUS_LABELS } from "@baudoku/shared-constants";

const statusOptions = Object.entries(PROJECT_STATUS_LABELS).map(
  ([value, label]) => ({ value, label })
);

export function ProjectListPage() {
  const { data: projects, isLoading } = useProjects();
  const deleteProject = useDeleteProject();
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<string | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const filtered = useMemo(() => {
    if (!projects) return [];
    return projects.filter((p) => {
      if (statusFilter && p.status !== statusFilter) return false;
      if (search) {
        const q = search.toLowerCase();
        return (
          p.name.toLowerCase().includes(q) ||
          p.city?.toLowerCase().includes(q) ||
          p.clientName?.toLowerCase().includes(q)
        );
      }
      return true;
    });
  }, [projects, search, statusFilter]);

  if (isLoading) {
    return <LoadingSkeleton />;
  }

  return (
    <div>
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Projekte</h1>
          <p className="mt-1 text-sm text-gray-500">
            {projects?.length ?? 0} Projekte gesamt
          </p>
        </div>
        <Link
          to="/projects/new"
          className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-blue-700 transition-colors"
        >
          <PlusIcon />
          Neues Projekt
        </Link>
      </div>

      <div className="mt-6 space-y-4">
        <SearchBar
          value={search}
          onChange={setSearch}
          placeholder="Projekte suchen..."
        />
        <FilterChips
          options={statusOptions}
          selected={statusFilter}
          onChange={setStatusFilter}
        />
      </div>

      {filtered.length === 0 ? (
        <div className="mt-8">
          <EmptyState
            title={search || statusFilter ? "Keine Treffer" : "Noch keine Projekte"}
            description={
              search || statusFilter
                ? "Versuchen Sie eine andere Suche oder Filter."
                : "Erstellen Sie Ihr erstes Projekt."
            }
            action={
              !search && !statusFilter ? (
                <Link
                  to="/projects/new"
                  className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
                >
                  <PlusIcon />
                  Projekt erstellen
                </Link>
              ) : undefined
            }
          />
        </div>
      ) : (
        <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {filtered.map((project) => (
            <div
              key={project.id}
              className="group relative rounded-xl border border-gray-200 bg-white p-5 hover:border-blue-300 hover:shadow-sm transition-all"
            >
              <Link
                to="/projects/$projectId"
                params={{ projectId: project.id }}
                className="block"
              >
                <h3 className="font-semibold text-gray-900 group-hover:text-blue-700">
                  {project.name}
                </h3>
                {(project.street || project.city) && (
                  <p className="mt-1 text-sm text-gray-500">
                    {[project.street, project.zipCode, project.city]
                      .filter(Boolean)
                      .join(", ")}
                  </p>
                )}
                {project.clientName && (
                  <p className="mt-1 text-sm text-gray-400">
                    {project.clientName}
                  </p>
                )}
                <StatusBadge status={project.status} className="mt-3" />
              </Link>

              <button
                onClick={(e) => {
                  e.preventDefault();
                  setDeleteId(project.id);
                }}
                className="absolute right-3 top-3 rounded p-1 text-gray-300 opacity-0 hover:text-red-500 group-hover:opacity-100 transition-opacity"
                title="Löschen"
              >
                <TrashIcon />
              </button>
            </div>
          ))}
        </div>
      )}

      <ConfirmDialog
        open={deleteId !== null}
        title="Projekt löschen"
        message="Möchten Sie dieses Projekt wirklich löschen? Alle zugehörigen Daten werden unwiderruflich entfernt."
        confirmLabel="Löschen"
        variant="danger"
        onConfirm={() => {
          if (deleteId) {
            deleteProject.mutate(deleteId);
            setDeleteId(null);
          }
        }}
        onCancel={() => setDeleteId(null)}
      />
    </div>
  );
}

function LoadingSkeleton() {
  return (
    <div>
      <div className="h-8 w-48 animate-pulse rounded bg-gray-200" />
      <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {Array.from({ length: 6 }).map((_, i) => (
          <div
            key={i}
            className="h-32 animate-pulse rounded-xl border border-gray-200 bg-gray-100"
          />
        ))}
      </div>
    </div>
  );
}

function PlusIcon() {
  return (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
    </svg>
  );
}

function TrashIcon() {
  return (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
      <path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
    </svg>
  );
}
