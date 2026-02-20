import {
  useInfiniteQuery,
  useQuery,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import { apiGet, apiPost, apiPut, apiDelete } from "@baudoku/core";
import type { PagedResult } from "@baudoku/core";
import type { Project, Zone, ProjectFormData, ZoneFormData } from "@baudoku/projects";

// ─── Projects ───────────────────────────────────────────────────

const PAGE_SIZE = 20;

export function useProjects(search?: string) {
  return useInfiniteQuery({
    queryKey: ["projects", { search }],
    queryFn: async ({ pageParam = 1 }) => {
      const params = new URLSearchParams({
        page: String(pageParam),
        pageSize: String(PAGE_SIZE),
      });
      if (search) params.set("search", search);
      return apiGet<PagedResult<Project>>(`/api/projects?${params}`);
    },
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
  });
}

export function useProjectStats() {
  return useQuery({
    queryKey: ["projects", "stats"],
    queryFn: () => apiGet<PagedResult<Project>>("/api/projects?page=1&pageSize=1"),
  });
}

export function useProject(projectId: string) {
  return useQuery({
    queryKey: ["projects", projectId],
    queryFn: () => apiGet<Project>(`/api/projects/${projectId}`),
    enabled: !!projectId,
  });
}

export function useCreateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: ProjectFormData) =>
      apiPost<Project>("/api/projects", data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
    },
  });
}

export function useUpdateProject(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: ProjectFormData) =>
      apiPut<Project>(`/api/projects/${projectId}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
    },
  });
}

export function useDeleteProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (projectId: string) =>
      apiDelete(`/api/projects/${projectId}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
    },
  });
}

// ─── Zones ──────────────────────────────────────────────────────

export function useZones(projectId: string) {
  return useQuery({
    queryKey: ["projects", projectId, "zones"],
    queryFn: () => apiGet<Zone[]>(`/api/projects/${projectId}/zones`),
    enabled: !!projectId,
  });
}

export function useCreateZone(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: ZoneFormData) =>
      apiPost<Zone>(`/api/projects/${projectId}/zones`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["projects", projectId, "zones"],
      });
    },
  });
}

export function useDeleteZone(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (zoneId: string) =>
      apiDelete(`/api/projects/${projectId}/zones/${zoneId}`),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["projects", projectId, "zones"],
      });
    },
  });
}
