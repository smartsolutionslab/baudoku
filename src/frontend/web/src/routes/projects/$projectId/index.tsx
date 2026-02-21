import { Link, useParams } from "@tanstack/react-router";
import { useState } from "react";
import {
  useProject,
  useZones,
  useDeleteZone,
  useUpdateProject,
} from "@/hooks/useProjects";
import { ZoneTree } from "@/components/projects/ZoneTree";
import { StatusBadge } from "@/components/common/StatusBadge";
import { ConfirmDialog } from "@/components/common/ConfirmDialog";
import { PlusIcon } from "@/components/icons";

export function ProjectDetailPage() {
  const { projectId } = useParams({ strict: false }) as { projectId: string };
  const { data: project, isLoading: projectLoading } = useProject(projectId);
  const { data: zones, isLoading: zonesLoading } = useZones(projectId);
  const deleteZone = useDeleteZone(projectId);
  const [deleteZoneId, setDeleteZoneId] = useState<string | null>(null);

  if (projectLoading) {
    return (
      <div className="space-y-4">
        <div className="h-8 w-64 animate-pulse rounded bg-gray-200" />
        <div className="h-48 animate-pulse rounded-xl bg-gray-100" />
      </div>
    );
  }

  if (!project) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Projekt nicht gefunden.</p>
        <Link to="/projects" className="mt-2 text-sm text-blue-600 hover:underline">
          Zurück zur Übersicht
        </Link>
      </div>
    );
  }

  return (
    <div>
      {/* Header */}
      <div className="flex items-start justify-between">
        <div>
          <Link
            to="/projects"
            className="text-sm text-gray-500 hover:text-gray-700"
          >
            &larr; Alle Projekte
          </Link>
          <h1 className="mt-2 text-2xl font-bold text-gray-900">
            {project.name}
          </h1>
          <div className="mt-2 flex items-center gap-3">
            <StatusBadge status={project.status} />
            {project.city && (
              <span className="text-sm text-gray-500">
                {[project.street, project.zipCode, project.city]
                  .filter(Boolean)
                  .join(", ")}
              </span>
            )}
          </div>
        </div>
        <Link
          to="/projects/$projectId/installations"
          params={{ projectId }}
          className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-blue-700"
        >
          Installationen
        </Link>
      </div>

      {/* Project info */}
      {project.clientName && (
        <div className="mt-6 rounded-xl border border-gray-200 bg-white p-5">
          <h2 className="text-sm font-semibold text-gray-900">Auftraggeber</h2>
          <p className="mt-1 text-sm text-gray-600">{project.clientName}</p>
          {project.clientContact && (
            <p className="text-sm text-gray-500">{project.clientContact}</p>
          )}
        </div>
      )}

      {/* Zone tree */}
      <div className="mt-6">
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold text-gray-900">Zonen</h2>
          <Link
            to="/projects/$projectId/zones/new"
            params={{ projectId }}
            className="inline-flex items-center gap-1 rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            <PlusIcon />
            Zone hinzufügen
          </Link>
        </div>
        <div className="mt-3 rounded-xl border border-gray-200 bg-white p-4">
          {zonesLoading ? (
            <div className="space-y-2">
              {Array.from({ length: 3 }).map((_, i) => (
                <div key={i} className="h-8 animate-pulse rounded bg-gray-100" />
              ))}
            </div>
          ) : (
            <ZoneTree
              zones={zones ?? []}
              projectId={projectId}
              onDelete={(id) => setDeleteZoneId(id)}
            />
          )}
        </div>
      </div>

      <ConfirmDialog
        open={deleteZoneId !== null}
        title="Zone löschen"
        message="Möchten Sie diese Zone wirklich löschen?"
        confirmLabel="Löschen"
        variant="danger"
        onConfirm={() => {
          if (deleteZoneId) {
            deleteZone.mutate(deleteZoneId);
            setDeleteZoneId(null);
          }
        }}
        onCancel={() => setDeleteZoneId(null)}
      />
    </div>
  );
}

