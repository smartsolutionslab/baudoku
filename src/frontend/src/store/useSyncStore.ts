import { create } from "zustand";
import * as syncRepo from "../db/repositories/syncRepo";
import type { SyncOutboxEntry } from "../db/repositories/types";

interface SyncState {
  unsyncedCount: number;
  isOnline: boolean;
  lastSyncTimestamp: string | null;
  pendingEntries: SyncOutboxEntry[];
  loading: boolean;

  setOnline: (online: boolean) => void;
  loadSyncStatus: () => Promise<void>;
  loadPendingEntries: () => Promise<void>;
}

export const useSyncStore = create<SyncState>((set) => ({
  unsyncedCount: 0,
  isOnline: true,
  lastSyncTimestamp: null,
  pendingEntries: [],
  loading: false,

  setOnline: (online) => set({ isOnline: online }),

  loadSyncStatus: async () => {
    set({ loading: true });
    const [unsyncedCount, lastSyncTimestamp] = await Promise.all([
      syncRepo.getUnsyncedCount(),
      syncRepo.getLastSyncTimestamp(),
    ]);
    set({ unsyncedCount, lastSyncTimestamp, loading: false });
  },

  loadPendingEntries: async () => {
    const [pending, failed] = await Promise.all([
      syncRepo.getPendingEntries(),
      syncRepo.getFailedEntries(),
    ]);
    const pendingEntries = [...pending, ...failed];
    set({ pendingEntries, unsyncedCount: pendingEntries.length });
  },
}));
