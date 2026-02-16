import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import * as zoneRepo from "../db/repositories/zoneRepo";
import type { NewZone } from "../db/repositories/types";
import type { ProjectId, ZoneId } from "../types/branded";

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
    mutationFn: (data: Omit<NewZone, "id" | "version">) => zoneRepo.create(data),
    meta: { errorMessage: "Zone konnte nicht erstellt werden" },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: ["zones", variables.projectId] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}

export function useUpdateZone() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: ZoneId; data: Partial<Omit<NewZone, "id" | "version" | "projectId">> }) => zoneRepo.update(id, data),
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
    mutationFn: (id: ZoneId) => zoneRepo.remove(id), meta: { errorMessage: "Zone konnte nicht gelöscht werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["zones"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}
