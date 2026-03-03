import * as measurementRepo from '../db/repositories/measurementRepo';
import type { NewMeasurement } from '../db/repositories/types';
import type { InstallationId, MeasurementId } from '../types/branded';
import { useListQuery, useSyncMutation } from './useQueryFactory';

export function useMeasurementsByInstallation(installationId: InstallationId) {
  return useListQuery(['measurements', installationId], () => measurementRepo.getByInstallationId(installationId), !!installationId);
}

export function useAddMeasurement() {
  return useSyncMutation<Omit<NewMeasurement, 'id' | 'version' | 'result'>>({
    mutationFn: (data) => measurementRepo.create(data),
    errorMessage: 'Messung konnte nicht hinzugefügt werden',
    invalidateKeys: [['measurements']],
    onSuccessKeys: (variables) => [['measurements', variables.installationId]],
  });
}

export function useDeleteMeasurement() {
  return useSyncMutation<MeasurementId>({
    mutationFn: (id) => measurementRepo.remove(id),
    errorMessage: 'Messung konnte nicht gelöscht werden',
    invalidateKeys: [['measurements']],
  });
}
