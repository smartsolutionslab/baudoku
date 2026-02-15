import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import * as measurementRepo from "../db/repositories/measurementRepo";
import type { NewMeasurement } from "../db/repositories/types";
import type { InstallationId, MeasurementId } from "../types/branded";

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
