import * as photoRepo from '../db/repositories/photoRepo';
import type { NewPhoto } from '../db/repositories/types';
import type { InstallationId, PhotoId } from '@baudoku/core';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function usePhotosByInstallation(installationId: InstallationId) {
  return useListQuery(
    ['photos', installationId],
    () => photoRepo.getByInstallationId(installationId),
    !!installationId,
  );
}

export function useAddPhoto() {
  return useSyncMutation({
    mutationFn: (data: Omit<NewPhoto, 'id' | 'version'>) => photoRepo.create(data),
    errorMessage: 'Foto konnte nicht hinzugefügt werden',
    invalidateKeys: [['photos']],
    onSuccessKeys: (variables) => [['photos', variables.installationId]],
  });
}

export function useDeletePhoto() {
  return useSyncMutation({
    mutationFn: (id: PhotoId) => photoRepo.remove(id),
    errorMessage: 'Foto konnte nicht gelöscht werden',
    invalidateKeys: [['photos']],
  });
}

export function useUpdatePhotoAnnotation() {
  return useSyncMutation({
    mutationFn: ({ id, annotation }: { id: PhotoId; annotation: string }) =>
      photoRepo.updateAnnotation(id, annotation),
    errorMessage: 'Foto-Anmerkung konnte nicht aktualisiert werden',
    invalidateKeys: [['photos']],
  });
}
