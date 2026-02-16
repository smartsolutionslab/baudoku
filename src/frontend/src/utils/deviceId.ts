import { db } from "../db/client";
import { syncMeta } from "../db/schema";
import { eq } from "drizzle-orm";
import { generateId } from "./uuid";

let cachedDeviceId: string | null = null;

export async function getDeviceId(): Promise<string> {
  if (cachedDeviceId) return cachedDeviceId;

  const row = await db
    .select()
    .from(syncMeta)
    .where(eq(syncMeta.key, "deviceId"))
    .get();

  if (row) {
    cachedDeviceId = row.value;
    return row.value;
  }

  const newId = generateId();
  await db.insert(syncMeta).values({ key: "deviceId", value: newId });
  cachedDeviceId = newId;
  return newId;
}
