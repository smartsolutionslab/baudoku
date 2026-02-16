import { useSyncStore } from "../store";
import { useSyncStatusQuery } from "./useOfflineData";

export function useSyncStatus() {
  const { isOnline } = useSyncStore();
  const { data } = useSyncStatusQuery();

  return {
    isOnline,
    unsyncedCount: data?.unsyncedCount ?? 0,
    pendingUploads: data?.pendingUploads ?? 0,
    lastSyncTimestamp: data?.lastSyncTimestamp ?? null,
  };
}
