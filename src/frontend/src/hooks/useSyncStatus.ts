import { useEffect } from "react";
import { useSyncStore } from "../store/useSyncStore";
import { useSyncStatusQuery } from "./useOfflineData";

export function useSyncStatus() {
  const { isOnline, setOnline } = useSyncStore();
  const { data } = useSyncStatusQuery();

  return {
    isOnline,
    unsyncedCount: data?.unsyncedCount ?? 0,
    lastSyncTimestamp: data?.lastSyncTimestamp ?? null,
  };
}
