import { create } from "zustand";
import * as syncRepo from "../db/repositories/syncRepo";
import type { SyncOutboxEntry } from "../db/repositories/types";
import type { SyncResult } from "../sync/SyncManager";
import type { ConflictDto } from "../sync/syncApi";

interface SyncState {
  unsyncedCount: number;
  isOnline: boolean;
  lastSyncTimestamp: string | null;
  pendingEntries: SyncOutboxEntry[];
  loading: boolean;

  isSyncing: boolean;
  lastSyncResult: SyncResult | null;
  syncError: string | null;
  conflicts: ConflictDto[];

  setOnline: (online: boolean) => void;
  loadSyncStatus: () => Promise<void>;
  loadPendingEntries: () => Promise<void>;
  startSync: () => void;
  setSyncResult: (result: SyncResult) => void;
  setSyncError: (error: string | null) => void;
  setConflicts: (conflicts: ConflictDto[]) => void;
}

export const useSyncStore = create<SyncState>((set) => ({
  unsyncedCount: 0,
  isOnline: true,
  lastSyncTimestamp: null,
  pendingEntries: [],
  loading: false,

  isSyncing: false,
  lastSyncResult: null,
  syncError: null,
  conflicts: [],

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

  startSync: () => set({ isSyncing: true, syncError: null }),

  setSyncResult: (result) =>
    set({
      isSyncing: false,
      lastSyncResult: result,
      syncError: result.errors.length > 0 ? result.errors.join("; ") : null,
    }),

  setSyncError: (error) => set({ isSyncing: false, syncError: error }),

  setConflicts: (conflicts) => set({ conflicts }),
}));
