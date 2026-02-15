import { useCallback } from "react";
import { useSyncContext } from "../providers/SyncProvider";
import { useSyncStore } from "../store/useSyncStore";
import { useToastStore } from "../store/useToastStore";
import type { SyncResult } from "../sync/SyncManager";

export function useSyncManager() {
  const { syncManager, syncScheduler } = useSyncContext();
  const {
    isSyncing,
    lastSyncResult,
    syncError,
    startSync,
    setSyncResult,
    setSyncError,
    loadSyncStatus,
    loadPendingEntries,
  } = useSyncStore();

  const sync = useCallback(async (): Promise<SyncResult | null> => {
    startSync();
    try {
      const result = await syncManager.sync();
      setSyncResult(result);
      void loadSyncStatus();
      void loadPendingEntries();
      const total = result.pushed + result.pulled;
      if (total > 0) {
        useToastStore.getState().show(
          `${total} Änderung${total !== 1 ? "en" : ""} synchronisiert`,
          "success",
        );
      }
      return result;
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Unbekannter Fehler";
      setSyncError(message);
      useToastStore.getState().show(`Sync fehlgeschlagen: ${message}`, "error");
      return null;
    }
  }, [syncManager, startSync, setSyncResult, setSyncError, loadSyncStatus, loadPendingEntries]);

  const push = useCallback(async () => {
    startSync();
    try {
      const result = await syncManager.push();
      void loadSyncStatus();
      void loadPendingEntries();
      return result;
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Unbekannter Fehler";
      setSyncError(message);
      return null;
    }
  }, [syncManager, startSync, setSyncError, loadSyncStatus, loadPendingEntries]);

  const pull = useCallback(async () => {
    startSync();
    try {
      const result = await syncManager.pull();
      void loadSyncStatus();
      return result;
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Unbekannter Fehler";
      setSyncError(message);
      return null;
    }
  }, [syncManager, startSync, setSyncError, loadSyncStatus]);

  const triggerNow = useCallback(() => {
    void syncScheduler?.triggerNow();
  }, [syncScheduler]);

  return {
    sync,
    push,
    pull,
    triggerNow,
    isSyncing,
    lastSyncResult,
    syncError,
  };
}
