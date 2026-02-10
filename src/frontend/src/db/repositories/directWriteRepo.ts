import { eq } from "drizzle-orm";
import { db } from "../client";
import {
  projects,
  zones,
  installations,
  photos,
  measurements,
} from "../schema";

/**
 * Direct writes for pull-path (server deltas).
 * These bypass the outbox to avoid echo-loops.
 */

type TableMap = {
  project: typeof projects;
  zone: typeof zones;
  installation: typeof installations;
  photo: typeof photos;
  measurement: typeof measurements;
};

const tableMap: TableMap = {
  project: projects,
  zone: zones,
  installation: installations,
  photo: photos,
  measurement: measurements,
};

export async function applyUpsert(
  entityType: string,
  entityId: string,
  payload: Record<string, unknown>
): Promise<void> {
  const table = tableMap[entityType as keyof TableMap];
  if (!table) {
    console.warn(`Unknown entity type for direct write: ${entityType}`);
    return;
  }

  const existing = await db
    .select()
    .from(table)
    .where(eq(table.id, entityId))
    .get();

  if (existing) {
    await db.update(table).set(payload).where(eq(table.id, entityId));
  } else {
    await db.insert(table).values({ ...payload, id: entityId } as never);
  }
}

export async function applyDelete(
  entityType: string,
  entityId: string
): Promise<void> {
  const table = tableMap[entityType as keyof TableMap];
  if (!table) {
    console.warn(`Unknown entity type for direct delete: ${entityType}`);
    return;
  }

  await db.delete(table).where(eq(table.id, entityId));
}
