import { useState, useMemo } from 'react';
import { Link, useParams } from '@tanstack/react-router';
import { projectId as toProjectId } from '@baudoku/core';
import type { InstallationId, ZoneId } from '@baudoku/core';
import { useInstallations, useDeleteInstallation, useZones, useConfirmDelete } from '@/hooks';
import {
  StatusBadge,
  SearchBar,
  FilterChips,
  EmptyState,
  ConfirmDialog,
  LoadingSkeleton,
  buttonClassName,
} from '@/components/common';
import { PlusIcon, TrashIcon } from '@/components/icons';
import { INSTALLATION_STATUS_LABELS } from '@baudoku/documentation';
import { optionsFromLabels } from '@baudoku/core';

const statusOptions = optionsFromLabels(INSTALLATION_STATUS_LABELS);

export function InstallationListPage() {
  const { projectId: rawProjectId } = useParams({ strict: false }) as { projectId: string };
  const projectId = toProjectId(rawProjectId);
  const { data: installations, isLoading } = useInstallations(projectId);
  const { data: zones } = useZones(projectId);
  const deleteInstallation = useDeleteInstallation(projectId);

  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<string | null>(null);
  const { requestDelete, confirmProps } = useConfirmDelete<InstallationId>((id) =>
    deleteInstallation.mutate(id),
  );

  const zoneMap = useMemo(() => {
    const map = new Map<ZoneId, string>();
    zones?.forEach(({ id, name }) => map.set(id, name));
    return map;
  }, [zones]);

  const filtered = useMemo(() => {
    if (!installations) return [];
    return installations.filter(({ status, type, manufacturer, serialNumber, zoneId }) => {
      if (statusFilter && status !== statusFilter) return false;
      if (search) {
        const q = search.toLowerCase();
        return (
          type.toLowerCase().includes(q) ||
          manufacturer?.toLowerCase().includes(q) ||
          serialNumber?.toLowerCase().includes(q) ||
          zoneMap.get(zoneId)?.toLowerCase().includes(q)
        );
      }
      return true;
    });
  }, [installations, search, statusFilter, zoneMap]);

  if (isLoading) {
    return <LoadingSkeleton count={4} itemClassName="h-20" layout="list" />;
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
          <h1 className="mt-2 text-2xl font-bold text-gray-900">Installationen</h1>
          <p className="mt-1 text-sm text-gray-500">{installations?.length ?? 0} Installationen</p>
        </div>
        <Link
          to="/projects/$projectId/installations/new"
          params={{ projectId }}
          className={buttonClassName.primary}
        >
          <PlusIcon />
          Neue Installation
        </Link>
      </div>

      <div className="mt-6 space-y-4">
        <SearchBar value={search} onChange={setSearch} placeholder="Installationen suchen..." />
        <FilterChips options={statusOptions} selected={statusFilter} onChange={setStatusFilter} />
      </div>

      {filtered.length === 0 ? (
        <div className="mt-8">
          <EmptyState
            title={search || statusFilter ? 'Keine Treffer' : 'Noch keine Installationen'}
            description={
              search || statusFilter
                ? 'Versuchen Sie eine andere Suche oder Filter.'
                : 'Dokumentieren Sie Ihre erste Installation.'
            }
            action={
              !search && !statusFilter ? (
                <Link
                  to="/projects/$projectId/installations/new"
                  params={{ projectId }}
                  className={buttonClassName.primary}
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
          {filtered.map(({ id, type, zoneId, manufacturer, serialNumber, status }) => (
            <Link
              key={id}
              to="/projects/$projectId/installations/$installationId"
              params={{
                projectId,
                installationId: id,
              }}
              className="group block rounded-xl border border-gray-200 bg-white p-4 hover:border-blue-300 hover:shadow-sm transition-all"
            >
              <div className="flex items-start justify-between">
                <div className="flex-1 min-w-0">
                  <h3 className="font-medium text-gray-900 group-hover:text-blue-700">{type}</h3>
                  <div className="mt-1 flex flex-wrap items-center gap-2 text-sm text-gray-500">
                    <span>{zoneMap.get(zoneId) ?? '—'}</span>
                    {manufacturer && (
                      <>
                        <span className="text-gray-300">|</span>
                        <span>{manufacturer}</span>
                      </>
                    )}
                    {serialNumber && (
                      <>
                        <span className="text-gray-300">|</span>
                        <span>SN: {serialNumber}</span>
                      </>
                    )}
                  </div>
                </div>
                <div className="ml-4 flex items-center gap-2">
                  <StatusBadge status={status} />
                  <button
                    onClick={(e) => {
                      e.preventDefault();
                      requestDelete(id);
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
        {...confirmProps}
        title="Installation löschen"
        message="Möchten Sie diese Installation wirklich löschen?"
        confirmLabel="Löschen"
        variant="danger"
      />
    </div>
  );
}

