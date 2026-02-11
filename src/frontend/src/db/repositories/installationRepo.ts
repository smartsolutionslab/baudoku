import { eq, like, or, inArray, sql, count } from "drizzle-orm";
import { db } from "../client";
import { installations, zones, projects } from "../schema";
import { generateId } from "../../utils/uuid";
import { createOutboxEntry } from "./syncRepo";
import type { Installation, NewInstallation } from "./types";

export async function getByZoneId(zoneId: string): Promise<Installation[]> {
  return db
    .select()
    .from(installations)
    .where(eq(installations.zoneId, zoneId))
    .all();
}

export async function getByProjectId(
  projectId: string
): Promise<Installation[]> {
  return db
    .select()
    .from(installations)
    .where(eq(installations.projectId, projectId))
    .all();
}

export async function getById(
  id: string
): Promise<Installation | undefined> {
  return db
    .select()
    .from(installations)
    .where(eq(installations.id, id))
    .get();
}

export async function create(
  data: Omit<NewInstallation, "id" | "createdAt" | "updatedAt" | "version">
): Promise<Installation> {
  const now = new Date();
  const installation: NewInstallation = {
    ...data,
    id: generateId(),
    createdAt: now,
    updatedAt: now,
    version: 1,
  };

  await db.insert(installations).values(installation);
  await createOutboxEntry(
    "installation",
    installation.id,
    "create",
    installation
  );

  return installation as Installation;
}

export async function update(
  id: string,
  data: Partial<
    Omit<
      NewInstallation,
      "id" | "createdAt" | "updatedAt" | "version" | "projectId" | "zoneId"
    >
  >
): Promise<Installation | undefined> {
  const existing = await getById(id);
  if (!existing) return undefined;

  const updated = {
    ...data,
    updatedAt: new Date(),
    version: existing.version + 1,
  };

  await db
    .update(installations)
    .set(updated)
    .where(eq(installations.id, id));
  await createOutboxEntry("installation", id, "update", {
    ...existing,
    ...updated,
  });

  return { ...existing, ...updated } as Installation;
}

export async function remove(id: string): Promise<void> {
  await db.delete(installations).where(eq(installations.id, id));
  await createOutboxEntry("installation", id, "delete", { id });
}

export interface SearchResult extends Installation {
  zoneName: string;
  projectName: string;
}

export async function search(
  query: string,
  filters?: { status?: string[]; projectId?: string }
): Promise<SearchResult[]> {
  const pattern = `%${query}%`;

  let q = db
    .select({
      installation: installations,
      zoneName: zones.name,
      projectName: projects.name,
    })
    .from(installations)
    .innerJoin(zones, eq(installations.zoneId, zones.id))
    .innerJoin(projects, eq(installations.projectId, projects.id))
    .$dynamic();

  const conditions = [];

  if (query.trim()) {
    conditions.push(
      or(
        like(installations.type, pattern),
        like(installations.manufacturer, pattern),
        like(installations.model, pattern),
        like(installations.serialNumber, pattern),
        like(installations.notes, pattern)
      )
    );
  }

  if (filters?.status && filters.status.length > 0) {
    conditions.push(
      inArray(
        installations.status,
        filters.status as ("planned" | "in_progress" | "completed" | "inspected")[]
      )
    );
  }

  if (filters?.projectId) {
    conditions.push(eq(installations.projectId, filters.projectId));
  }

  if (conditions.length > 0) {
    for (const cond of conditions) {
      if (cond) q = q.where(cond);
    }
  }

  const rows = await q.all();
  return rows.map((r) => ({
    ...r.installation,
    zoneName: r.zoneName,
    projectName: r.projectName,
  }));
}

export async function getCountByStatus(): Promise<Record<string, number>> {
  const rows = await db
    .select({
      status: installations.status,
      count: count(),
    })
    .from(installations)
    .groupBy(installations.status)
    .all();

  const result: Record<string, number> = {};
  for (const r of rows) {
    result[r.status] = r.count;
  }
  return result;
}

export async function getAll(): Promise<Installation[]> {
  return db.select().from(installations).all();
}
