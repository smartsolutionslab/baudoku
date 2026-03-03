import { useInfiniteQuery } from '@tanstack/react-query';
import { apiGet } from '@baudoku/core';
import type { PagedResult } from '@baudoku/core';
import type { Project, Zone, ProjectFormData, ZoneFormData } from '@baudoku/projects';
import { useApiQuery, useApiPost, useApiPut, useApiDelete } from './useApiFactory';

// ─── Projects ───────────────────────────────────────────────────

const PAGE_SIZE = 20;

export function useProjects(search?: string) {
  return useInfiniteQuery({
    queryKey: ['projects', { search }],
    queryFn: async ({ pageParam = 1 }) => {
      const params = new URLSearchParams({
        page: String(pageParam),
        pageSize: String(PAGE_SIZE),
      });
      if (search) params.set('search', search);
      return apiGet<PagedResult<Project>>(`/api/projects?${params}`);
    },
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
  });
}

export function useProject(projectId: string) {
  return useApiQuery<Project>(['projects', projectId], `/api/projects/${projectId}`, !!projectId);
}

export function useCreateProject() {
  return useApiPost<Project, ProjectFormData>('/api/projects', [['projects']]);
}

export function useUpdateProject(projectId: string) {
  return useApiPut<Project, ProjectFormData>(`/api/projects/${projectId}`, [['projects']]);
}

export function useDeleteProject() {
  return useApiDelete((id) => `/api/projects/${id}`, [['projects']]);
}

// ─── Zones ──────────────────────────────────────────────────────

export function useZones(projectId: string) {
  return useApiQuery<Zone[]>(['projects', projectId, 'zones'], `/api/projects/${projectId}/zones`, !!projectId);
}

export function useCreateZone(projectId: string) {
  return useApiPost<Zone, ZoneFormData>(`/api/projects/${projectId}/zones`, [['projects', projectId, 'zones']]);
}

export function useDeleteZone(projectId: string) {
  return useApiDelete((zoneId) => `/api/projects/${projectId}/zones/${zoneId}`, [['projects', projectId, 'zones']]);
}
