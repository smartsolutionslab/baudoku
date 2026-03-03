import * as photoRepo from '../db/repositories/photoRepo';
import type { NewPhoto } from '../db/repositories/types';
import type { InstallationId, PhotoId } from '../types/branded';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function usePhotosByInstallation(installationId: InstallationId) {
  return useListQuery(['photos', installationId], () => photoRepo.getByInstallationId(installationId), !!installationId);
}

export function useAddPhoto() {
  return useSyncMutation<Omit<NewPhoto, 'id' | 'version'>>({
    mutationFn: (data) => photoRepo.create(data),
    errorMessage: 'Foto konnte nicht hinzugefügt werden',
    invalidateKeys: [['photos']],
    onSuccessKeys: (variables) => [['photos', variables.installationId]],
  });
}

export function useDeletePhoto() {
  return useSyncMutation<PhotoId>({
    mutationFn: (id) => photoRepo.remove(id),
    errorMessage: 'Foto konnte nicht gelöscht werden',
    invalidateKeys: [['photos']],
  });
}

export function useUpdatePhotoAnnotation() {
  return useSyncMutation<{ id: PhotoId; annotation: string }>({
    mutationFn: ({ id, annotation }) => photoRepo.updateAnnotation(id, annotation),
    errorMessage: 'Foto-Anmerkung konnte nicht aktualisiert werden',
    invalidateKeys: [['photos']],
  });
}
