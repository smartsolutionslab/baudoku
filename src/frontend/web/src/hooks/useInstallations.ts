import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { apiGet, apiPost, apiPut, apiDelete } from "@baudoku/core";
import type {
  Installation,
  Photo,
  Measurement,
  InstallationFormData,
  MeasurementFormData,
} from "@baudoku/documentation";

// ─── Installations ──────────────────────────────────────────────

export function useInstallations(projectId: string) {
  return useQuery({
    queryKey: ["projects", projectId, "installations"],
    queryFn: () =>
      apiGet<Installation[]>(
        `/api/documentation/installations?projectId=${projectId}`
      ),
    enabled: !!projectId,
  });
}

export function useInstallation(installationId: string) {
  return useQuery({
    queryKey: ["installations", installationId],
    queryFn: () =>
      apiGet<Installation>(
        `/api/documentation/installations/${installationId}`
      ),
    enabled: !!installationId,
  });
}

export function useCreateInstallation(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: InstallationFormData & { zoneId: string }) =>
      apiPost<Installation>(
        `/api/documentation/installations`,
        { ...data, projectId }
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["projects", projectId, "installations"],
      });
    },
  });
}

export function useUpdateInstallation(
  installationId: string,
  projectId: string
) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: InstallationFormData) =>
      apiPut<Installation>(
        `/api/documentation/installations/${installationId}`,
        data
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["installations", installationId],
      });
      queryClient.invalidateQueries({
        queryKey: ["projects", projectId, "installations"],
      });
    },
  });
}

export function useDeleteInstallation(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (installationId: string) =>
      apiDelete(
        `/api/documentation/installations/${installationId}`
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["projects", projectId, "installations"],
      });
    },
  });
}

// ─── Photos ─────────────────────────────────────────────────────

export function usePhotos(installationId: string) {
  return useQuery({
    queryKey: ["installations", installationId, "photos"],
    queryFn: () =>
      apiGet<Photo[]>(
        `/api/documentation/installations/${installationId}/photos`
      ),
    enabled: !!installationId,
  });
}

export function useUploadPhoto(installationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({
      file,
      caption,
    }: {
      file: File;
      caption?: string;
    }) => {
      const formData = new FormData();
      formData.append("file", file);
      if (caption) formData.append("caption", caption);

      const response = await fetch(
        `/api/documentation/installations/${installationId}/photos`,
        {
          method: "POST",
          body: formData,
        }
      );
      if (!response.ok) throw new Error("Upload fehlgeschlagen");
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["installations", installationId, "photos"],
      });
    },
  });
}

export function useDeletePhoto(installationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (photoId: string) =>
      apiDelete(
        `/api/documentation/installations/${installationId}/photos/${photoId}`
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["installations", installationId, "photos"],
      });
    },
  });
}

// ─── Measurements ───────────────────────────────────────────────

export function useMeasurements(installationId: string) {
  return useQuery({
    queryKey: ["installations", installationId, "measurements"],
    queryFn: () =>
      apiGet<Measurement[]>(
        `/api/documentation/installations/${installationId}/measurements`
      ),
    enabled: !!installationId,
  });
}

export function useCreateMeasurement(installationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: MeasurementFormData) =>
      apiPost<Measurement>(
        `/api/documentation/installations/${installationId}/measurements`,
        data
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["installations", installationId, "measurements"],
      });
    },
  });
}

export function useDeleteMeasurement(installationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (measurementId: string) =>
      apiDelete(
        `/api/documentation/installations/${installationId}/measurements/${measurementId}`
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["installations", installationId, "measurements"],
      });
    },
  });
}
