import { useParams } from '@tanstack/react-router';
import { projectId as toProjectId, installationId as toInstallationId } from '@baudoku/core';
import type { ProjectId, InstallationId } from '@baudoku/core';

export function useProjectIdParam(): ProjectId {
  const { projectId } = useParams({ strict: false }) as { projectId: string };
  return toProjectId(projectId);
}

export function useInstallationIdParam(): InstallationId {
  const { installationId } = useParams({ strict: false }) as { installationId: string };
  return toInstallationId(installationId);
}
