import * as zoneRepo from '../db/repositories/zoneRepo';
import type { NewZone } from '../db/repositories/types';
import type { ProjectId, ZoneId } from '@baudoku/core';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function useZonesByProject(projectId: ProjectId) {
  return useListQuery(['zones', projectId], () => zoneRepo.getByProjectId(projectId), !!projectId);
}

export function useCreateZone() {
  return useSyncMutation({
    mutationFn: (data: Omit<NewZone, 'id' | 'version'>) => zoneRepo.create(data),
    errorMessage: 'Zone konnte nicht erstellt werden',
    invalidateKeys: [['zones']],
    onSuccessKeys: (variables) => [['zones', variables.projectId]],
  });
}

export function useUpdateZone() {
  return useSyncMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: ZoneId;
      data: Partial<Omit<NewZone, 'id' | 'version' | 'projectId'>>;
    }) => zoneRepo.update(id, data),
    errorMessage: 'Zone konnte nicht aktualisiert werden',
    invalidateKeys: [['zones']],
  });
}

export function useDeleteZone() {
  return useSyncMutation({
    mutationFn: (id: ZoneId) => zoneRepo.remove(id),
    errorMessage: 'Zone konnte nicht gelöscht werden',
    invalidateKeys: [['zones']],
  });
}
