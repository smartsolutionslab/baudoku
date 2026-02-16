import { and, eq, like, or, inArray, sql, count } from "drizzle-orm";
import { db } from "../client";
import { installations, zones, projects } from "../schema";
import { generateId } from "../../utils";
import { createOutboxEntry } from "./syncRepo";
import type { Installation, NewInstallation } from "./types";
import type { InstallationId, ProjectId, ZoneId, ProjectName, ZoneName } from "../../types/branded";

export async function getByZoneId(zoneId: ZoneId): Promise<Installation[]> {
  return db
    .select()
    .from(installations)
    .where(eq(installations.zoneId, zoneId))
    .all() as unknown as Installation[];
}

export async function getByProjectId(
  projectId: ProjectId
): Promise<Installation[]> {
  return db
    .select()
    .from(installations)
    .where(eq(installations.projectId, projectId))
    .all() as unknown as Installation[];
}

export async function getById(
  id: InstallationId
): Promise<Installation | undefined> {
  return db
    .select()
    .from(installations)
    .where(eq(installations.id, id))
    .get() as unknown as Installation | undefined;
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

  return installation as unknown as Installation;
}

export async function update(
  id: InstallationId,
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

export async function remove(id: InstallationId): Promise<void> {
  await db.delete(installations).where(eq(installations.id, id));
  await createOutboxEntry("installation", id, "delete", { id });
}

export type SearchResult = Installation & {
  zoneName: ZoneName;
  projectName: ProjectName;
};

export async function search(
  query: string,
  filters?: { status?: string[]; projectId?: ProjectId }
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
    q = q.where(and(...conditions));
  }

  const rows = await q.all();
  return rows.map((r) => ({
    ...(r.installation as unknown as Installation),
    zoneName: r.zoneName as ZoneName,
    projectName: r.projectName as ProjectName,
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
  return db.select().from(installations).all() as unknown as Installation[];
}
