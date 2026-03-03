import * as projectRepo from '../db/repositories/projectRepo';
import type { NewProject } from '../db/repositories/types';
import type { ProjectId } from '../types/branded';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function useProjects() {
  return useListQuery(['projects'], () => projectRepo.getAll());
}

export function useProject(id: ProjectId) {
  return useListQuery(['projects', id], () => projectRepo.getById(id));
}

export function useCreateProject() {
  return useSyncMutation<Omit<NewProject, 'id' | 'createdAt' | 'updatedAt' | 'version'>>({
    mutationFn: (data) => projectRepo.create(data),
    errorMessage: 'Projekt konnte nicht erstellt werden',
    invalidateKeys: [['projects']],
  });
}

export function useUpdateProject() {
  return useSyncMutation<{ id: ProjectId; data: Partial<Omit<NewProject, 'id' | 'createdAt' | 'updatedAt' | 'version' | 'createdBy'>> }>({
    mutationFn: ({ id, data }) => projectRepo.update(id, data),
    errorMessage: 'Projekt konnte nicht aktualisiert werden',
    invalidateKeys: [['projects']],
  });
}

export function useDeleteProject() {
  return useSyncMutation<ProjectId>({
    mutationFn: (id) => projectRepo.remove(id),
    errorMessage: 'Projekt konnte nicht gelöscht werden',
    invalidateKeys: [['projects']],
  });
}
