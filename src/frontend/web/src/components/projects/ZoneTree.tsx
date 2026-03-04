import { useMemo } from 'react';
import { Link } from '@tanstack/react-router';
import type { ProjectId, ZoneId } from '@baudoku/core';
import { buildZoneTree, flattenTree, ZONE_TYPE_LABELS } from '@baudoku/projects';
import type { Zone, ZoneNode } from '@baudoku/projects';
import { PlusIcon, TrashIcon, CornerIcon, ListIcon } from '@/components/icons';

interface ZoneTreeProps {
  zones: Zone[];
  projectId: ProjectId;
  onDelete?: (zoneId: ZoneId) => void;
}

export function ZoneTree({ zones, projectId, onDelete }: ZoneTreeProps) {
  const tree = useMemo(() => buildZoneTree(zones), [zones]);
  const flat = useMemo(() => flattenTree(tree), [tree]);

  if (flat.length === 0) {
    return (
      <div className='rounded-lg border-2 border-dashed border-gray-300 p-8 text-center'>
        <p className='text-sm text-gray-500'>Noch keine Zonen angelegt.</p>
        <Link
          to='/projects/$projectId/zones/new'
          params={{ projectId }}
          className='mt-3 inline-flex items-center gap-1 text-sm font-medium text-blue-600 hover:text-blue-700'
        >
          <PlusIcon />
          Zone hinzufügen
        </Link>
      </div>
    );
  }

  return (
    <div className='space-y-1'>
      {flat.map((node) => (
        <ZoneRow
          key={node.zone.id}
          node={node}
          projectId={projectId}
          onDelete={onDelete}
        />
      ))}
    </div>
  );
}

function ZoneRow({
  node,
  projectId,
  onDelete,
}: {
  node: ZoneNode;
  projectId: ProjectId;
  onDelete?: (zoneId: ZoneId) => void;
}) {
  const { zone, level } = node;

  return (
    <div
      className='group flex items-center gap-3 rounded-lg px-3 py-2 hover:bg-gray-50'
      style={{ paddingLeft: `${level * 24 + 12}px` }}
    >
      {level > 0 && (
        <span className='text-gray-300'>
          <CornerIcon />
        </span>
      )}
      <div className='flex-1 min-w-0'>
        <span className='text-sm font-medium text-gray-900'>{zone.name}</span>
        <span className='ml-2 text-xs text-gray-400'>
          {ZONE_TYPE_LABELS[zone.type as keyof typeof ZONE_TYPE_LABELS] ?? zone.type}
        </span>
      </div>
      <div className='flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity'>
        <Link
          to='/projects/$projectId/installations'
          params={{ projectId }}
          search={{ zoneId: zone.id }}
          className='rounded p-1 text-gray-400 hover:text-blue-600'
          title='Installationen anzeigen'
        >
          <ListIcon />
        </Link>
        {onDelete && (
          <button
            onClick={() => onDelete(zone.id)}
            className='rounded p-1 text-gray-400 hover:text-red-500'
            title='Zone löschen'
          >
            <TrashIcon />
          </button>
        )}
      </div>
    </div>
  );
}

