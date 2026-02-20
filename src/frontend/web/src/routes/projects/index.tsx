import { useState, useDeferredValue, useMemo } from "react";
import { Link } from "@tanstack/react-router";
import { useProjects, useDeleteProject } from "@/hooks/useProjects";
import { StatusBadge } from "@/components/common/StatusBadge";
import { SearchBar } from "@/components/common/SearchBar";
import { FilterChips } from "@/components/common/FilterChips";
import { EmptyState } from "@/components/common/EmptyState";
import { ConfirmDialog } from "@/components/common/ConfirmDialog";
import { PlusIcon, TrashIcon } from "@/components/icons";
import { PROJECT_STATUS_LABELS } from "@baudoku/projects";

const statusOptions = Object.entries(PROJECT_STATUS_LABELS).map(
  ([value, label]) => ({ value, label })
);

export function ProjectListPage() {
  const [search, setSearch] = useState("");
  const deferredSearch = useDeferredValue(search);
  const {
    data,
    isLoading,
    hasNextPage,
    fetchNextPage,
    isFetchingNextPage,
  } = useProjects(deferredSearch || undefined);
  const deleteProject = useDeleteProject();
  const [statusFilter, setStatusFilter] = useState<string | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const allProjects = useMemo(
    () => data?.pages.flatMap((p) => p.items) ?? [],
    [data]
  );

  const totalCount = data?.pages[0]?.totalCount ?? 0;

  const filtered = useMemo(() => {
    if (!statusFilter) return allProjects;
    return allProjects.filter((p) => p.status === statusFilter);
  }, [allProjects, statusFilter]);

  if (isLoading) {
    return <LoadingSkeleton />;
  }

  return (
    <div>
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Projekte</h1>
          <p className="mt-1 text-sm text-gray-500">
            {totalCount} Projekte gesamt
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
        <>
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

          {hasNextPage && (
            <div className="mt-6 flex justify-center">
              <button
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
                className="inline-flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-5 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 transition-colors"
              >
                {isFetchingNextPage ? "Laden..." : "Mehr laden"}
              </button>
            </div>
          )}
        </>
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
