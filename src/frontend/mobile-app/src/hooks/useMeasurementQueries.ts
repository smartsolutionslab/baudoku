import * as measurementRepo from '../db/repositories/measurementRepo';
import type { NewMeasurement } from '../db/repositories/types';
import type { InstallationId, MeasurementId } from '@baudoku/core';
import { MUTATION_ERRORS } from '../constants/strings';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function useMeasurementsByInstallation(installationId: InstallationId) {
  return useListQuery(
    ['measurements', installationId],
    () => measurementRepo.getByInstallationId(installationId),
    !!installationId,
  );
}

export function useAddMeasurement() {
  return useSyncMutation({
    mutationFn: (data: Omit<NewMeasurement, 'id' | 'version' | 'result'>) =>
      measurementRepo.create(data),
    errorMessage: MUTATION_ERRORS.measurementAdd,
    invalidateKeys: [['measurements']],
    onSuccessKeys: (variables) => [['measurements', variables.installationId]],
  });
}

export function useDeleteMeasurement() {
  return useSyncMutation({
    mutationFn: (id: MeasurementId) => measurementRepo.remove(id),
    errorMessage: MUTATION_ERRORS.measurementDelete,
    invalidateKeys: [['measurements']],
  });
}
