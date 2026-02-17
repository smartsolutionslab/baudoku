import { useQuery } from "@tanstack/react-query";
import * as syncRepo from "../db/repositories/syncRepo";
import * as photoRepo from "../db/repositories/photoRepo";

export function useSyncStatusQuery() {
  return useQuery({
    queryKey: ["syncStatus"],
    queryFn: async () => {
      const [unsyncedCount, lastSyncTimestamp, pendingUploads] =
        await Promise.all([
          syncRepo.getUnsyncedCount(),
          syncRepo.getLastSyncTimestamp(),
          photoRepo.getPendingUploadCount(),
        ]);
      return { unsyncedCount, lastSyncTimestamp, pendingUploads };
    },
  });
}
