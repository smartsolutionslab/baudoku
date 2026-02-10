import { eq, and } from "drizzle-orm";
import { db } from "../client";
import { zones } from "../schema";
import { generateId } from "../../utils/uuid";
import { createOutboxEntry } from "./syncRepo";
import type { Zone, NewZone } from "./types";

export async function getByProjectId(projectId: string): Promise<Zone[]> {
  return db
    .select()
    .from(zones)
    .where(eq(zones.projectId, projectId))
    .all();
}

export async function getById(id: string): Promise<Zone | undefined> {
  return db.select().from(zones).where(eq(zones.id, id)).get();
}

export async function getChildren(parentZoneId: string): Promise<Zone[]> {
  return db
    .select()
    .from(zones)
    .where(eq(zones.parentZoneId, parentZoneId))
    .all();
}

export async function getRootZones(projectId: string): Promise<Zone[]> {
  return db
    .select()
    .from(zones)
    .where(
      and(
        eq(zones.projectId, projectId),
        eq(zones.parentZoneId, ""),
      )
    )
    .all();
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

  return zone as Zone;
}

export async function update(
  id: string,
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

export async function remove(id: string): Promise<void> {
  await db.delete(zones).where(eq(zones.id, id));
  await createOutboxEntry("zone", id, "delete", { id });
}
