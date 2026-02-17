import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { apiGet, apiPost, apiPut, apiDelete } from "@baudoku/shared-api";
import type { Project, Zone } from "@baudoku/shared-types";
import type { ProjectFormData, ZoneFormData } from "@baudoku/shared-validation";

// ─── Projects ───────────────────────────────────────────────────

export function useProjects() {
  return useQuery({
    queryKey: ["projects"],
    queryFn: () => apiGet<Project[]>("/api/projects"),
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
