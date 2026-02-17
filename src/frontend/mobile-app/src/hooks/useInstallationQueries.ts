import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import * as installationRepo from "../db/repositories/installationRepo";
import type { NewInstallation } from "../db/repositories/types";
import type { ProjectId, ZoneId, InstallationId } from "../types/branded";

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
    mutationFn: (data: Omit<NewInstallation, "id" | "createdAt" | "updatedAt" | "version">) => installationRepo.create(data),
    meta: { errorMessage: "Installation konnte nicht erstellt werden" },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: ["installations", "zone", variables.zoneId] });
      queryClient.invalidateQueries({ queryKey: ["installations", "project", variables.projectId] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useUpdateInstallation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: {
      id: InstallationId;
      data: Partial<Omit<NewInstallation, "id" | "createdAt" | "updatedAt" | "version" | "projectId" | "zoneId">>;
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
