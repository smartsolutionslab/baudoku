import * as projectRepo from '../db/repositories/projectRepo';
import type { NewProject } from '../db/repositories/types';
import type { ProjectId } from '@baudoku/core';
import { MUTATION_ERRORS } from '../constants/strings';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function useProjects() {
  return useListQuery(['projects'], () => projectRepo.getAll());
}

export function useProject(id: ProjectId) {
  return useListQuery(['projects', id], () => projectRepo.getById(id));
}

export function useCreateProject() {
  return useSyncMutation({
    mutationFn: (data: Omit<NewProject, 'id' | 'createdAt' | 'updatedAt' | 'version'>) =>
      projectRepo.create(data),
    errorMessage: MUTATION_ERRORS.projectCreate,
    invalidateKeys: [['projects']],
  });
}

export function useUpdateProject() {
  return useSyncMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: ProjectId;
      data: Partial<Omit<NewProject, 'id' | 'createdAt' | 'updatedAt' | 'version' | 'createdBy'>>;
    }) => projectRepo.update(id, data),
    errorMessage: MUTATION_ERRORS.projectUpdate,
    invalidateKeys: [['projects']],
  });
}

export function useDeleteProject() {
  return useSyncMutation({
    mutationFn: (id: ProjectId) => projectRepo.remove(id),
    errorMessage: MUTATION_ERRORS.projectDelete,
    invalidateKeys: [['projects']],
  });
}
