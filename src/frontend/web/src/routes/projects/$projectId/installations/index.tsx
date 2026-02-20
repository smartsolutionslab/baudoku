import { useState, useMemo } from "react";
import { Link, useParams, useSearch } from "@tanstack/react-router";
import {
  useInstallations,
  useDeleteInstallation,
} from "@/hooks/useInstallations";
import { useZones } from "@/hooks/useProjects";
import { StatusBadge } from "@/components/common/StatusBadge";
import { SearchBar } from "@/components/common/SearchBar";
import { FilterChips } from "@/components/common/FilterChips";
import { EmptyState } from "@/components/common/EmptyState";
import { ConfirmDialog } from "@/components/common/ConfirmDialog";
import { PlusIcon, TrashIcon } from "@/components/icons";
import { INSTALLATION_STATUS_LABELS } from "@baudoku/documentation";

const statusOptions = Object.entries(INSTALLATION_STATUS_LABELS).map(
  ([value, label]) => ({ value, label })
);

export function InstallationListPage() {
  const { projectId } = useParams({ strict: false }) as { projectId: string };
  const { data: installations, isLoading } = useInstallations(projectId);
  const { data: zones } = useZones(projectId);
  const deleteInstallation = useDeleteInstallation(projectId);

  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<string | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const zoneMap = useMemo(() => {
    const map = new Map<string, string>();
    zones?.forEach((z) => map.set(z.id, z.name));
    return map;
  }, [zones]);

  const filtered = useMemo(() => {
    if (!installations) return [];
    return installations.filter((inst) => {
      if (statusFilter && inst.status !== statusFilter) return false;
      if (search) {
        const q = search.toLowerCase();
        return (
          inst.type.toLowerCase().includes(q) ||
          inst.manufacturer?.toLowerCase().includes(q) ||
          inst.serialNumber?.toLowerCase().includes(q) ||
          zoneMap.get(inst.zoneId)?.toLowerCase().includes(q)
        );
      }
      return true;
    });
  }, [installations, search, statusFilter, zoneMap]);

  if (isLoading) {
    return <LoadingSkeleton />;
  }

  return (
    <div>
      <div className="flex items-center justify-between">
        <div>
          <Link
            to="/projects/$projectId"
            params={{ projectId }}
            className="text-sm text-gray-500 hover:text-gray-700"
          >
            &larr; Zurück zum Projekt
          </Link>
          <h1 className="mt-2 text-2xl font-bold text-gray-900">
            Installationen
          </h1>
          <p className="mt-1 text-sm text-gray-500">
            {installations?.length ?? 0} Installationen
          </p>
        </div>
        <Link
          to="/projects/$projectId/installations/new"
          params={{ projectId }}
          className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-blue-700"
        >
          <PlusIcon />
          Neue Installation
        </Link>
      </div>

      <div className="mt-6 space-y-4">
        <SearchBar
          value={search}
          onChange={setSearch}
          placeholder="Installationen suchen..."
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
            title={
              search || statusFilter
                ? "Keine Treffer"
                : "Noch keine Installationen"
            }
            description={
              search || statusFilter
                ? "Versuchen Sie eine andere Suche oder Filter."
                : "Dokumentieren Sie Ihre erste Installation."
            }
            action={
              !search && !statusFilter ? (
                <Link
                  to="/projects/$projectId/installations/new"
                  params={{ projectId }}
                  className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
                >
                  <PlusIcon />
                  Installation erstellen
                </Link>
              ) : undefined
            }
          />
        </div>
      ) : (
        <div className="mt-6 space-y-3">
          {filtered.map((inst) => (
            <Link
              key={inst.id}
              to="/projects/$projectId/installations/$installationId"
              params={{
                projectId,
                installationId: inst.id,
              }}
              className="group block rounded-xl border border-gray-200 bg-white p-4 hover:border-blue-300 hover:shadow-sm transition-all"
            >
              <div className="flex items-start justify-between">
                <div className="flex-1 min-w-0">
                  <h3 className="font-medium text-gray-900 group-hover:text-blue-700">
                    {inst.type}
                  </h3>
                  <div className="mt-1 flex flex-wrap items-center gap-2 text-sm text-gray-500">
                    <span>{zoneMap.get(inst.zoneId) ?? "—"}</span>
                    {inst.manufacturer && (
                      <>
                        <span className="text-gray-300">|</span>
                        <span>{inst.manufacturer}</span>
                      </>
                    )}
                    {inst.serialNumber && (
                      <>
                        <span className="text-gray-300">|</span>
                        <span>SN: {inst.serialNumber}</span>
                      </>
                    )}
                  </div>
                </div>
                <div className="ml-4 flex items-center gap-2">
                  <StatusBadge status={inst.status} />
                  <button
                    onClick={(e) => {
                      e.preventDefault();
                      setDeleteId(inst.id);
                    }}
                    className="rounded p-1 text-gray-300 opacity-0 hover:text-red-500 group-hover:opacity-100 transition-opacity"
                    title="Löschen"
                  >
                    <TrashIcon />
                  </button>
                </div>
              </div>
            </Link>
          ))}
        </div>
      )}

      <ConfirmDialog
        open={deleteId !== null}
        title="Installation löschen"
        message="Möchten Sie diese Installation wirklich löschen?"
        confirmLabel="Löschen"
        variant="danger"
        onConfirm={() => {
          if (deleteId) {
            deleteInstallation.mutate(deleteId);
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
      <div className="mt-6 space-y-3">
        {Array.from({ length: 4 }).map((_, i) => (
          <div
            key={i}
            className="h-20 animate-pulse rounded-xl border border-gray-200 bg-gray-100"
          />
        ))}
      </div>
    </div>
  );
}

