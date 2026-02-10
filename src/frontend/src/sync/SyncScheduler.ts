import { SyncManager } from "./SyncManager";
import type { SyncResult } from "./SyncManager";

const DEFAULT_INTERVAL_MS = 60_000;

export class SyncScheduler {
  private readonly syncManager: SyncManager;
  private readonly intervalMs: number;
  private intervalId: ReturnType<typeof setInterval> | null = null;
  private onSyncComplete?: (result: SyncResult) => void;
  private onSyncError?: (error: Error) => void;

  constructor(
    syncManager: SyncManager,
    options?: {
      intervalMs?: number;
      onSyncComplete?: (result: SyncResult) => void;
      onSyncError?: (error: Error) => void;
    }
  ) {
    this.syncManager = syncManager;
    this.intervalMs = options?.intervalMs ?? DEFAULT_INTERVAL_MS;
    this.onSyncComplete = options?.onSyncComplete;
    this.onSyncError = options?.onSyncError;
  }

  start(): void {
    if (this.intervalId) return;

    this.intervalId = setInterval(() => {
      void this.runSync();
    }, this.intervalMs);
  }

  stop(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }

  async triggerNow(): Promise<SyncResult | null> {
    return this.runSync();
  }

  private async runSync(): Promise<SyncResult | null> {
    try {
      const result = await this.syncManager.sync();
      this.onSyncComplete?.(result);
      return result;
    } catch (error) {
      const err = error instanceof Error ? error : new Error(String(error));
      this.onSyncError?.(err);
      return null;
    }
  }
}
