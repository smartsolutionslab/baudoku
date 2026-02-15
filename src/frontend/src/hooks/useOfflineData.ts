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
import type {
  ProjectId,
  ZoneId,
  InstallationId,
  PhotoId,
  MeasurementId,
} from "../types/branded";

// ─── Projects ────────────────────────────────────────────────────

export function useProjects() {
  return useQuery({
    queryKey: ["projects"],
    queryFn: () => projectRepo.getAll(),
  });
}

export function useProject(id: ProjectId) {
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
    meta: { errorMessage: "Projekt konnte nicht erstellt werden" },
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
      id: ProjectId;
      data: Partial<
        Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version" | "createdBy">
      >;
    }) => projectRepo.update(id, data),
    meta: { errorMessage: "Projekt konnte nicht aktualisiert werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useDeleteProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: ProjectId) => projectRepo.remove(id),
    meta: { errorMessage: "Projekt konnte nicht gelöscht werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Zones ───────────────────────────────────────────────────────

export function useZonesByProject(projectId: ProjectId) {
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
    meta: { errorMessage: "Zone konnte nicht erstellt werden" },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["zones", variables.projectId],
      });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useUpdateZone() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: ZoneId;
      data: Partial<Omit<NewZone, "id" | "version" | "projectId">>;
    }) => zoneRepo.update(id, data),
    meta: { errorMessage: "Zone konnte nicht aktualisiert werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["zones"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useDeleteZone() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: ZoneId) => zoneRepo.remove(id),
    meta: { errorMessage: "Zone konnte nicht gelöscht werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["zones"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Installations ───────────────────────────────────────────────

export function useInstallationsByZone(zoneId: ZoneId) {
  return useQuery({
    queryKey: ["installations", "zone", zoneId],
    queryFn: () => installationRepo.getByZoneId(zoneId),
    enabled: !!zoneId,
  });
}

export function useInstallationsByProject(projectId: ProjectId) {
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
    meta: { errorMessage: "Installation konnte nicht erstellt werden" },
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

export function useUpdateInstallation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: InstallationId;
      data: Partial<
        Omit<
          NewInstallation,
          "id" | "createdAt" | "updatedAt" | "version" | "projectId" | "zoneId"
        >
      >;
    }) => installationRepo.update(id, data),
    meta: { errorMessage: "Installation konnte nicht aktualisiert werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["installations"] });
      queryClient.invalidateQueries({ queryKey: ["installation"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useDeleteInstallation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: InstallationId) => installationRepo.remove(id),
    meta: { errorMessage: "Installation konnte nicht gelöscht werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["installations"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Photos ──────────────────────────────────────────────────────

export function usePhotosByInstallation(installationId: InstallationId) {
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
    meta: { errorMessage: "Foto konnte nicht hinzugefügt werden" },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["photos", variables.installationId],
      });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useDeletePhoto() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: PhotoId) => photoRepo.remove(id),
    meta: { errorMessage: "Foto konnte nicht gelöscht werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["photos"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useUpdatePhotoAnnotation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, annotation }: { id: PhotoId; annotation: string }) =>
      photoRepo.updateAnnotation(id, annotation),
    meta: { errorMessage: "Foto-Anmerkung konnte nicht aktualisiert werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["photos"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Measurements ────────────────────────────────────────────────

export function useMeasurementsByInstallation(installationId: InstallationId) {
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
    meta: { errorMessage: "Messung konnte nicht hinzugefügt werden" },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["measurements", variables.installationId],
      });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useDeleteMeasurement() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: MeasurementId) => measurementRepo.remove(id),
    meta: { errorMessage: "Messung konnte nicht gelöscht werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["measurements"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

// ─── Sync Status ─────────────────────────────────────────────────

export function useSyncStatusQuery() {
  return useQuery({
    queryKey: ["syncStatus"],
    queryFn: async () => {
      const [unsyncedCount, lastSyncTimestamp, pendingUploads] =
        await Promise.all([
          syncRepo.getUnsyncedCount(),
          syncRepo.getLastSyncTimestamp(),
          photoRepo.getPendingUploadCount(),
        ]);
      return { unsyncedCount, lastSyncTimestamp, pendingUploads };
    },
  });
}
