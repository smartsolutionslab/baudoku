import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiGet, apiPost } from '@baudoku/core';
import type { PagedResult, ProjectId, InstallationId } from '@baudoku/core';
import type {
  Installation,
  Photo,
  Measurement,
  InstallationFormData,
  MeasurementFormData,
} from '@baudoku/documentation';
import { uploadPhoto } from '@baudoku/documentation';
import { toBackendRequest, type CreateInstallationInput } from '@/mappers/installationMapper';
import { useApiQuery, useApiPost, useApiPut, useApiDelete, useApiMutation } from './useApiFactory';

// ─── Installations ──────────────────────────────────────────────

export function useInstallations(projectId: ProjectId) {
  return useQuery({
    queryKey: ['projects', projectId, 'installations'],
    queryFn: async () => {
      const result = await apiGet<PagedResult<Installation>>(
        `/api/documentation/installations?projectId=${projectId}`,
      );
      return result.items;
    },
    enabled: !!projectId,
  });
}

export function useInstallation(installationId: InstallationId) {
  return useApiQuery<Installation>(
    ['installations', installationId],
    `/api/documentation/installations/${installationId}`,
    !!installationId,
  );
}

export function useCreateInstallation(projectId: ProjectId) {
  return useApiMutation<CreateInstallationInput, Installation>({
    mutationFn: (data) =>
      apiPost<Installation>('/api/documentation/installations', toBackendRequest(data, projectId)),
    invalidateKeys: [['projects', projectId, 'installations']],
  });
}

export function useUpdateInstallation(installationId: InstallationId, projectId: ProjectId) {
  return useApiPut<Installation, InstallationFormData>(
    `/api/documentation/installations/${installationId}`,
    [
      ['installations', installationId],
      ['projects', projectId, 'installations'],
    ],
  );
}

export function useDeleteInstallation(projectId: ProjectId) {
  return useApiDelete(
    (id) => `/api/documentation/installations/${id}`,
    [['projects', projectId, 'installations']],
  );
}

// ─── Photos ─────────────────────────────────────────────────────

export function usePhotos(installationId: InstallationId) {
  return useApiQuery<Photo[]>(
    ['installations', installationId, 'photos'],
    `/api/documentation/installations/${installationId}/photos`,
    !!installationId,
  );
}

export function useUploadPhoto(installationId: InstallationId) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ file, caption }: { file: File; caption?: string }) =>
      uploadPhoto(installationId, file, caption),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['installations', installationId, 'photos'],
      });
    },
  });
}

export function useDeletePhoto(installationId: InstallationId) {
  return useApiDelete(
    (photoId) => `/api/documentation/installations/${installationId}/photos/${photoId}`,
    [['installations', installationId, 'photos']],
  );
}

// ─── Measurements ───────────────────────────────────────────────

export function useMeasurements(installationId: InstallationId) {
  return useApiQuery<Measurement[]>(
    ['installations', installationId, 'measurements'],
    `/api/documentation/installations/${installationId}/measurements`,
    !!installationId,
  );
}

export function useCreateMeasurement(installationId: InstallationId) {
  return useApiPost<Measurement, MeasurementFormData>(
    `/api/documentation/installations/${installationId}/measurements`,
    [['installations', installationId, 'measurements']],
  );
}

export function useDeleteMeasurement(installationId: InstallationId) {
  return useApiDelete(
    (measurementId) =>
      `/api/documentation/installations/${installationId}/measurements/${measurementId}`,
    [['installations', installationId, 'measurements']],
  );
}
