import { eq } from "drizzle-orm";
import { db } from "../client";
import { installations } from "../schema";
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
