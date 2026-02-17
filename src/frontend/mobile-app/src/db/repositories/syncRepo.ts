import { eq, and, sql } from "drizzle-orm";
import { db } from "../client";
import { syncOutbox, syncMeta } from "../schema";
import { generateId, getDeviceId } from "../../utils";
import type { SyncOutboxEntry } from "./types";

export async function createOutboxEntry(entityType: string, entityId: string, operation: "create" | "update" | "delete", payload: unknown): Promise<void> {
  const deviceId = await getDeviceId();
  await db.insert(syncOutbox).values({
    id: generateId(),
    entityType,
    entityId,
    operation,
    payload: JSON.stringify(payload),
    timestamp: new Date(),
    deviceId,
    retryCount: 0,
    status: "pending",
  });
}

export async function getPendingEntries(): Promise<SyncOutboxEntry[]> {
  return db.select().from(syncOutbox).where(and(eq(syncOutbox.status, "pending")))
    .all() as unknown as SyncOutboxEntry[];
}

export async function getFailedEntries(): Promise<SyncOutboxEntry[]> {
  return db.select().from(syncOutbox).where(eq(syncOutbox.status, "failed")).all() as unknown as SyncOutboxEntry[];
}

export async function getUnsyncedCount(): Promise<number> {
  const result = await db.select({ count: sql<number>`count(*)` }).from(syncOutbox).where(sql`${syncOutbox.status} IN ('pending', 'failed')`).get();
  return result?.count ?? 0;
}

export async function markAsSyncing(ids: string[]): Promise<void> {
  for (const id of ids) {
    await db.update(syncOutbox).set({ status: "syncing" }).where(eq(syncOutbox.id, id));
  }
}

export async function markAsSynced(ids: string[]): Promise<void> {
  for (const id of ids) {
    await db.update(syncOutbox).set({ status: "synced" }).where(eq(syncOutbox.id, id));
  }
}

export async function markAsFailed(ids: string[]): Promise<void> {
  for (const id of ids) {
    await db.update(syncOutbox).set({status: "failed", retryCount: sql`${syncOutbox.retryCount} + 1`}).where(eq(syncOutbox.id, id));
    }
}

export async function getLastSyncTimestamp(): Promise<string | null> {
  const row = await db.select().from(syncMeta).where(eq(syncMeta.key, "lastSyncTimestamp")).get();
  return row?.value ?? null;
}

export async function setLastSyncTimestamp(timestamp: string): Promise<void> {
  await db.insert(syncMeta).values({ key: "lastSyncTimestamp", value: timestamp })
    .onConflictDoUpdate({ target: syncMeta.key, set: { value: timestamp }});
}
