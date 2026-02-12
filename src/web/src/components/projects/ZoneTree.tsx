import { useMemo } from "react";
import { Link } from "@tanstack/react-router";
import type { Zone } from "@baudoku/shared-types";
import { buildZoneTree, flattenTree, type ZoneNode } from "@baudoku/shared-logic";
import { ZONE_TYPE_LABELS } from "@baudoku/shared-constants";

interface ZoneTreeProps {
  zones: Zone[];
  projectId: string;
  onDelete?: (zoneId: string) => void;
}

export function ZoneTree({ zones, projectId, onDelete }: ZoneTreeProps) {
  const tree = useMemo(() => buildZoneTree(zones), [zones]);
  const flat = useMemo(() => flattenTree(tree), [tree]);

  if (flat.length === 0) {
    return (
      <div className="rounded-lg border-2 border-dashed border-gray-300 p-8 text-center">
        <p className="text-sm text-gray-500">Noch keine Zonen angelegt.</p>
        <Link
          to="/projects/$projectId/zones/new"
          params={{ projectId }}
          className="mt-3 inline-flex items-center gap-1 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          <PlusIcon />
          Zone hinzufügen
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-1">
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
  projectId: string;
  onDelete?: (zoneId: string) => void;
}) {
  const { zone, level } = node;

  return (
    <div
      className="group flex items-center gap-3 rounded-lg px-3 py-2 hover:bg-gray-50"
      style={{ paddingLeft: `${level * 24 + 12}px` }}
    >
      {level > 0 && (
        <span className="text-gray-300">
          <CornerIcon />
        </span>
      )}
      <div className="flex-1 min-w-0">
        <span className="text-sm font-medium text-gray-900">{zone.name}</span>
        <span className="ml-2 text-xs text-gray-400">
          {ZONE_TYPE_LABELS[zone.type as keyof typeof ZONE_TYPE_LABELS] ?? zone.type}
        </span>
      </div>
      <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
        <Link
          to="/projects/$projectId/installations"
          params={{ projectId }}
          search={{ zoneId: zone.id }}
          className="rounded p-1 text-gray-400 hover:text-blue-600"
          title="Installationen anzeigen"
        >
          <ListIcon />
        </Link>
        {onDelete && (
          <button
            onClick={() => onDelete(zone.id)}
            className="rounded p-1 text-gray-400 hover:text-red-500"
            title="Zone löschen"
          >
            <TrashIcon />
          </button>
        )}
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

function CornerIcon() {
  return (
    <svg className="h-4 w-4" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth={1.5}>
      <path d="M4 2v6a2 2 0 0 0 2 2h6" />
    </svg>
  );
}

function ListIcon() {
  return (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 6.75h12M8.25 12h12m-12 5.25h12M3.75 6.75h.007v.008H3.75V6.75Zm.375 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0ZM3.75 12h.007v.008H3.75V12Zm.375 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm-.375 5.25h.007v.008H3.75v-.008Zm.375 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Z" />
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
