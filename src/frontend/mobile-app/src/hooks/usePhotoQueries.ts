import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import * as photoRepo from "../db/repositories/photoRepo";
import type { NewPhoto } from "../db/repositories/types";
import type { InstallationId, PhotoId } from "../types/branded";

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
    mutationFn: (data: Omit<NewPhoto, "id" | "version">) => photoRepo.create(data),
    meta: { errorMessage: "Foto konnte nicht hinzugefügt werden" },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: ["photos", variables.installationId] });
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
    mutationFn: ({ id, annotation }: { id: PhotoId; annotation: string }) => photoRepo.updateAnnotation(id, annotation),
    meta: { errorMessage: "Foto-Anmerkung konnte nicht aktualisiert werden" },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["photos"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}
