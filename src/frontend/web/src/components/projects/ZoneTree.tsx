import { useMemo } from "react";
import { Link } from "@tanstack/react-router";
import type { Zone } from "@baudoku/shared-types";
import { buildZoneTree, flattenTree, type ZoneNode } from "@baudoku/shared-logic";
import { ZONE_TYPE_LABELS } from "@baudoku/shared-constants";
import { PlusIcon, TrashIcon } from "@/components/icons";

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

