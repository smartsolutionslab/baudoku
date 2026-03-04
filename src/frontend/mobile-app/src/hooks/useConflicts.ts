import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import * as syncApi from '../sync/syncApi';
import { getDeviceId } from '../utils';
import { MUTATION_ERRORS } from '../constants/strings';
import type { ConflictDto } from '../sync/syncApi';

export function useConflicts(status?: string) {
  return useQuery({
    queryKey: ['conflicts', status],
    queryFn: async (): Promise<ConflictDto[]> => {
      const deviceId = await getDeviceId();
      return syncApi.getConflicts(deviceId, status);
    },
  });
}

export function useResolveConflict() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      conflictId,
      strategy,
      mergedPayload,
    }: {
      conflictId: string;
      strategy: string;
      mergedPayload?: string;
    }) => {
      await syncApi.resolveConflict(conflictId, strategy, mergedPayload);
    },
    meta: { errorMessage: MUTATION_ERRORS.conflictResolve },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['conflicts'] });
      queryClient.invalidateQueries({ queryKey: ['syncStatus'] });
    },
  });
}
