import { applyUpsert, applyDelete } from "../db/repositories/directWriteRepo";
import type { ServerDeltaDto } from "./syncApi";

/**
 * Applies a server delta to the local SQLite database.
 * Uses directWriteRepo (no outbox entries) to prevent echo-loops.
 */
export async function applyServerDelta(delta: ServerDeltaDto): Promise<void> {
  const { entityType, entityId, operation, payload } = delta;

  if (operation === "delete") {
    await applyDelete(entityType, entityId);
    return;
  }

  try {
    const parsed = JSON.parse(payload) as Record<string, unknown>;
    await applyUpsert(entityType, entityId, parsed);
  } catch (error) {
    console.error(
      `Failed to apply server delta for ${entityType}/${entityId}:`,
      error
    );
  }
}
