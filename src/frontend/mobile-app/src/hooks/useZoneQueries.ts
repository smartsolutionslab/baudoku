import * as zoneRepo from '../db/repositories/zoneRepo';
import type { NewZone } from '../db/repositories/types';
import type { ProjectId, ZoneId } from '@baudoku/core';
import { MUTATION_ERRORS } from '../constants/strings';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function useZonesByProject(projectId: ProjectId) {
  return useListQuery(['zones', projectId], () => zoneRepo.getByProjectId(projectId), !!projectId);
}

export function useCreateZone() {
  return useSyncMutation({
    mutationFn: (data: Omit<NewZone, 'id' | 'version'>) => zoneRepo.create(data),
    errorMessage: MUTATION_ERRORS.zoneCreate,
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
    errorMessage: MUTATION_ERRORS.zoneUpdate,
    invalidateKeys: [['zones']],
  });
}

export function useDeleteZone() {
  return useSyncMutation({
    mutationFn: (id: ZoneId) => zoneRepo.remove(id),
    errorMessage: MUTATION_ERRORS.zoneDelete,
    invalidateKeys: [['zones']],
  });
}
