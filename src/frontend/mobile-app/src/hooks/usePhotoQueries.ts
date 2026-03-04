import * as photoRepo from '../db/repositories/photoRepo';
import type { NewPhoto } from '../db/repositories/types';
import type { InstallationId, PhotoId } from '@baudoku/core';
import { MUTATION_ERRORS } from '../constants/strings';
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
    errorMessage: MUTATION_ERRORS.photoAdd,
    invalidateKeys: [['photos']],
    onSuccessKeys: (variables) => [['photos', variables.installationId]],
  });
}

export function useDeletePhoto() {
  return useSyncMutation({
    mutationFn: (id: PhotoId) => photoRepo.remove(id),
    errorMessage: MUTATION_ERRORS.photoDelete,
    invalidateKeys: [['photos']],
  });
}

export function useUpdatePhotoAnnotation() {
  return useSyncMutation({
    mutationFn: ({ id, annotation }: { id: PhotoId; annotation: string }) =>
      photoRepo.updateAnnotation(id, annotation),
    errorMessage: MUTATION_ERRORS.photoAnnotation,
    invalidateKeys: [['photos']],
  });
}
