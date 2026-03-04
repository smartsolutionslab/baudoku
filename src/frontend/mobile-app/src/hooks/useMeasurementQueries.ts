import * as measurementRepo from '../db/repositories/measurementRepo';
import type { NewMeasurement } from '../db/repositories/types';
import type { InstallationId, MeasurementId } from '@baudoku/core';
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
    errorMessage: 'Messung konnte nicht hinzugefügt werden',
    invalidateKeys: [['measurements']],
    onSuccessKeys: (variables) => [['measurements', variables.installationId]],
  });
}

export function useDeleteMeasurement() {
  return useSyncMutation({
    mutationFn: (id: MeasurementId) => measurementRepo.remove(id),
    errorMessage: 'Messung konnte nicht gelöscht werden',
    invalidateKeys: [['measurements']],
  });
}
