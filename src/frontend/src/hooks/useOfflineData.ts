import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import * as projectRepo from "../db/repositories/projectRepo";
import * as zoneRepo from "../db/repositories/zoneRepo";
import * as installationRepo from "../db/repositories/installationRepo";
import * as photoRepo from "../db/repositories/photoRepo";
import * as measurementRepo from "../db/repositories/measurementRepo";
import * as syncRepo from "../db/repositories/syncRepo";
import type {
  NewProject,
  NewZone,
  NewInstallation,
  NewPhoto,
  NewMeasurement,
} from "../db/repositories/types";

// ─── Projects ────────────────────────────────────────────────────

export function useProjects() {
  return useQuery({
    queryKey: ["projects"],
    queryFn: () => projectRepo.getAll(),
  });
}

export function useProject(id: string) {
  return useQuery({
    queryKey: ["projects", id],
    queryFn: () => projectRepo.getById(id),
  });
}

export function useCreateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (
      data: Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version">
    ) => projectRepo.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useUpdateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: string;
      data: Partial<
        Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version" | "createdBy">
      >;
    }) => projectRepo.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useDeleteProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => projectRepo.remove(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Zones ───────────────────────────────────────────────────────

export function useZonesByProject(projectId: string) {
  return useQuery({
    queryKey: ["zones", projectId],
    queryFn: () => zoneRepo.getByProjectId(projectId),
    enabled: !!projectId,
  });
}

export function useCreateZone() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: Omit<NewZone, "id" | "version">) =>
      zoneRepo.create(data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["zones", variables.projectId],
      });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Installations ───────────────────────────────────────────────

export function useInstallationsByZone(zoneId: string) {
  return useQuery({
    queryKey: ["installations", "zone", zoneId],
    queryFn: () => installationRepo.getByZoneId(zoneId),
    enabled: !!zoneId,
  });
}

export function useInstallationsByProject(projectId: string) {
  return useQuery({
    queryKey: ["installations", "project", projectId],
    queryFn: () => installationRepo.getByProjectId(projectId),
    enabled: !!projectId,
  });
}

export function useCreateInstallation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (
      data: Omit<NewInstallation, "id" | "createdAt" | "updatedAt" | "version">
    ) => installationRepo.create(data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["installations", "zone", variables.zoneId],
      });
      queryClient.invalidateQueries({
        queryKey: ["installations", "project", variables.projectId],
      });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Photos ──────────────────────────────────────────────────────

export function usePhotosByInstallation(installationId: string) {
  return useQuery({
    queryKey: ["photos", installationId],
    queryFn: () => photoRepo.getByInstallationId(installationId),
    enabled: !!installationId,
  });
}

export function useAddPhoto() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: Omit<NewPhoto, "id" | "version">) =>
      photoRepo.create(data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["photos", variables.installationId],
      });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Measurements ────────────────────────────────────────────────

export function useMeasurementsByInstallation(installationId: string) {
  return useQuery({
    queryKey: ["measurements", installationId],
    queryFn: () => measurementRepo.getByInstallationId(installationId),
    enabled: !!installationId,
  });
}

export function useAddMeasurement() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: Omit<NewMeasurement, "id" | "version" | "result">) =>
      measurementRepo.create(data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["measurements", variables.installationId],
      });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Sync Status ─────────────────────────────────────────────────

export function useSyncStatusQuery() {
  return useQuery({
    queryKey: ["syncStatus"],
    queryFn: async () => {
      const [unsyncedCount, lastSyncTimestamp] = await Promise.all([
        syncRepo.getUnsyncedCount(),
        syncRepo.getLastSyncTimestamp(),
      ]);
      return { unsyncedCount, lastSyncTimestamp };
    },
  });
}
