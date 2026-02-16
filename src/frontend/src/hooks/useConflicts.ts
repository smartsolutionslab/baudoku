import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import * as syncApi from "../sync/syncApi";
import { useSyncStore } from "../store/useSyncStore";
import { getDeviceId } from "../utils/deviceId";
import type { ConflictDto } from "../sync/syncApi";

export function useConflicts(status?: string) {
  const { setConflicts } = useSyncStore();

  return useQuery({
    queryKey: ["conflicts", status],
    queryFn: async (): Promise<ConflictDto[]> => {
      const deviceId = await getDeviceId();
      const conflicts = await syncApi.getConflicts(deviceId, status);
      setConflicts(conflicts);
      return conflicts;
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
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["conflicts"] });
      queryClient.invalidateQueries({ queryKey: ["syncStatus"] });
    },
  });
}
