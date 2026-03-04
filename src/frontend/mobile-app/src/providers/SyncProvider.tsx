import React, { useEffect, useRef, createContext, useContext } from 'react';
import { SyncManager } from '../sync/SyncManager';
import { SyncScheduler } from '../sync/SyncScheduler';
import { setOnReconnect } from '../sync/ConnectivityMonitor';
import { useSyncStore } from '../store';
import { UploadProgressBar } from '../components/sync';
import * as photoRepo from '../db/repositories/photoRepo';

type SyncContextValue = {
  syncManager: SyncManager;
  syncScheduler: SyncScheduler | null;
};

const SyncContext = createContext<SyncContextValue | null>(null);

export function useSyncContext(): SyncContextValue {
  const ctx = useContext(SyncContext);
  if (!ctx) {
    throw new Error('useSyncContext must be used within SyncProvider');
  }
  return ctx;
}

export function SyncProvider({ children }: { children: React.ReactNode }) {
  const managerRef = useRef<SyncManager>(new SyncManager());
  const schedulerRef = useRef<SyncScheduler | null>(null);

  useEffect(() => {
    void photoRepo.resetStuckUploads();

    const store = useSyncStore.getState();

    const scheduler = new SyncScheduler(managerRef.current, {
      intervalMs: 60_000,
      onSyncComplete: (result) => {
        store.setSyncResult(result);
        void store.loadSyncStatus();
        void store.loadPendingEntries();
      },
      onSyncError: (error) => {
        store.setSyncError(error.message);
      },
    });

    schedulerRef.current = scheduler;
    scheduler.start();

    setOnReconnect(() => {
      void scheduler.triggerNow();
    });

    return () => {
      scheduler.stop();
    };
  }, []);

  const value: SyncContextValue = {
    // eslint-disable-next-line react-hooks/refs
    syncManager: managerRef.current,
    // eslint-disable-next-line react-hooks/refs
    syncScheduler: schedulerRef.current,
  };

  return (
    // eslint-disable-next-line react-hooks/refs -- refs hold stable singleton instances for context
    <SyncContext.Provider value={value}>
      {children}
      <UploadProgressBar />
    </SyncContext.Provider>
  );
}
