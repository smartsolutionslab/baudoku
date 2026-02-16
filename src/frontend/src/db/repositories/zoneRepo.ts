import { eq } from "drizzle-orm";
import { db } from "../client";
import { zones } from "../schema";
import { generateId } from "../../utils/uuid";
import { createOutboxEntry } from "./syncRepo";
import type { Zone, NewZone } from "./types";
import type { ProjectId, ZoneId } from "../../types/branded";

export async function getByProjectId(projectId: ProjectId): Promise<Zone[]> {
  return db
    .select()
    .from(zones)
    .where(eq(zones.projectId, projectId))
    .all() as unknown as Zone[];
}

export async function getById(id: ZoneId): Promise<Zone | undefined> {
  return db.select().from(zones).where(eq(zones.id, id)).get() as unknown as Zone | undefined;
}

export async function create(
  data: Omit<NewZone, "id" | "version">
): Promise<Zone> {
  const zone: NewZone = {
    ...data,
    id: generateId(),
    version: 1,
  };

  await db.insert(zones).values(zone);
  await createOutboxEntry("zone", zone.id, "create", zone);

  return zone as unknown as Zone;
}

export async function update(
  id: ZoneId,
  data: Partial<Omit<NewZone, "id" | "version" | "projectId">>
): Promise<Zone | undefined> {
  const existing = await getById(id);
  if (!existing) return undefined;

  const updated = {
    ...data,
    version: existing.version + 1,
  };

  await db.update(zones).set(updated).where(eq(zones.id, id));
  await createOutboxEntry("zone", id, "update", { ...existing, ...updated });

  return { ...existing, ...updated } as Zone;
}

export async function remove(id: ZoneId): Promise<void> {
  await db.delete(zones).where(eq(zones.id, id));
  await createOutboxEntry("zone", id, "delete", { id });
}
