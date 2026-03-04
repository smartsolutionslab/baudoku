import * as installationRepo from '../db/repositories/installationRepo';
import type { NewInstallation } from '../db/repositories/types';
import type { ProjectId, ZoneId, InstallationId } from '@baudoku/core';
import { MUTATION_ERRORS } from '../constants/strings';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function useInstallationsByZone(zoneId: ZoneId) {
  return useListQuery(
    ['installations', 'zone', zoneId],
    () => installationRepo.getByZoneId(zoneId),
    !!zoneId,
  );
}

export function useInstallationsByProject(projectId: ProjectId) {
  return useListQuery(
    ['installations', 'project', projectId],
    () => installationRepo.getByProjectId(projectId),
    !!projectId,
  );
}

export function useCreateInstallation() {
  return useSyncMutation({
    mutationFn: (data: Omit<NewInstallation, 'id' | 'createdAt' | 'updatedAt' | 'version'>) =>
      installationRepo.create(data),
    errorMessage: MUTATION_ERRORS.installationCreate,
    invalidateKeys: [['installations']],
    onSuccessKeys: (variables) => [
      ['installations', 'zone', variables.zoneId],
      ['installations', 'project', variables.projectId],
    ],
  });
}

export function useUpdateInstallation() {
  return useSyncMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: InstallationId;
      data: Partial<
        Omit<NewInstallation, 'id' | 'createdAt' | 'updatedAt' | 'version' | 'projectId' | 'zoneId'>
      >;
    }) => installationRepo.update(id, data),
    errorMessage: MUTATION_ERRORS.installationUpdate,
    invalidateKeys: [['installations'], ['installation']],
  });
}

export function useDeleteInstallation() {
  return useSyncMutation({
    mutationFn: (id: InstallationId) => installationRepo.remove(id),
    errorMessage: MUTATION_ERRORS.installationDelete,
    invalidateKeys: [['installations']],
  });
}
