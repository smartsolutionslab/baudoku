import { useState, useDeferredValue, useMemo } from 'react';
import { Link } from '@tanstack/react-router';
import type { ProjectId } from '@baudoku/core';
import { useProjects, useDeleteProject, useConfirmDelete } from '@/hooks';
import { StatusBadge, SearchBar, FilterChips, EmptyState, ConfirmDialog, buttonClassName, Button } from '@/components/common';
import { PlusIcon, TrashIcon } from '@/components/icons';
import { PROJECT_STATUS_LABELS } from '@baudoku/projects';
import { optionsFromLabels } from '@baudoku/core';

const statusOptions = optionsFromLabels(PROJECT_STATUS_LABELS);

export function ProjectListPage() {
  const [search, setSearch] = useState('');
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
  const { requestDelete, confirmProps } = useConfirmDelete<ProjectId>((id) => deleteProject.mutate(id));

  const allProjects = useMemo(
    () => data?.pages.flatMap((p) => p.items) ?? [],
    [data]
  );

  const totalCount = data?.pages[0]?.totalCount ?? 0;

  const filtered = useMemo(() => {
    if (!statusFilter) return allProjects;
    return allProjects.filter(({ status }) => status === statusFilter);
  }, [allProjects, statusFilter]);

  if (isLoading) {
    return <LoadingSkeleton />;
  }

  return (
    <div>
      <div className='flex items-center justify-between'>
        <div>
          <h1 className='text-2xl font-bold text-gray-900'>Projekte</h1>
          <p className='mt-1 text-sm text-gray-500'>
            {totalCount} Projekte gesamt
          </p>
        </div>
        <Link
          to='/projects/new'
          className={buttonClassName.primary}
        >
          <PlusIcon />
          Neues Projekt
        </Link>
      </div>

      <div className='mt-6 space-y-4'>
        <SearchBar
          value={search}
          onChange={setSearch}
          placeholder='Projekte suchen...'
        />
        <FilterChips
          options={statusOptions}
          selected={statusFilter}
          onChange={setStatusFilter}
        />
      </div>

      {filtered.length === 0 ? (
        <div className='mt-8'>
          <EmptyState
            title={search || statusFilter ? 'Keine Treffer' : 'Noch keine Projekte'}
            description={
              search || statusFilter
                ? 'Versuchen Sie eine andere Suche oder Filter.'
                : 'Erstellen Sie Ihr erstes Projekt.'
            }
            action={
              !search && !statusFilter ? (
                <Link
                  to='/projects/new'
                  className={buttonClassName.primary}
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
          <div className='mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3'>
            {filtered.map(({ id, name, status, street, zipCode, city, clientName }) => (
              <div
                key={id}
                className='group relative rounded-xl border border-gray-200 bg-white p-5 hover:border-blue-300 hover:shadow-sm transition-all'
              >
                <Link
                  to='/projects/$projectId'
                  params={{ projectId: id }}
                  className='block'
                >
                  <h3 className='font-semibold text-gray-900 group-hover:text-blue-700'>
                    {name}
                  </h3>
                  {(street || city) && (
                    <p className='mt-1 text-sm text-gray-500'>
                      {[street, zipCode, city]
                        .filter(Boolean)
                        .join(', ')}
                    </p>
                  )}
                  {clientName && (
                    <p className='mt-1 text-sm text-gray-400'>
                      {clientName}
                    </p>
                  )}
                  <StatusBadge status={status} className='mt-3' />
                </Link>

                <button
                  onClick={(e) => {
                    e.preventDefault();
                    requestDelete(id);
                  }}
                  className='absolute right-3 top-3 rounded p-1 text-gray-300 opacity-0 hover:text-red-500 group-hover:opacity-100 transition-opacity'
                  title='Löschen'
                >
                  <TrashIcon />
                </button>
              </div>
            ))}
          </div>

          {hasNextPage && (
            <div className='mt-6 flex justify-center'>
              <Button
                variant='secondary'
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
              >
                {isFetchingNextPage ? 'Laden...' : 'Mehr laden'}
              </Button>
            </div>
          )}
        </>
      )}

      <ConfirmDialog
        {...confirmProps}
        title='Projekt löschen'
        message='Möchten Sie dieses Projekt wirklich löschen? Alle zugehörigen Daten werden unwiderruflich entfernt.'
        confirmLabel='Löschen'
        variant='danger'
      />
    </div>
  );
}

function LoadingSkeleton() {
  return (
    <div>
      <div className='h-8 w-48 animate-pulse rounded bg-gray-200' />
      <div className='mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3'>
        {Array.from({ length: 6 }).map((_, i) => (
          <div
            key={i}
            className='h-32 animate-pulse rounded-xl border border-gray-200 bg-gray-100'
          />
        ))}
      </div>
    </div>
  );
}
