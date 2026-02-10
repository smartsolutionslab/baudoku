import { eq } from "drizzle-orm";
import { db } from "../client";
import { projects } from "../schema";
import { generateId } from "../../utils/uuid";
import { createOutboxEntry } from "./syncRepo";
import type { Project, NewProject } from "./types";

export async function getAll(): Promise<Project[]> {
  return db.select().from(projects).all();
}

export async function getById(id: string): Promise<Project | undefined> {
  return db.select().from(projects).where(eq(projects.id, id)).get();
}

export async function getByStatus(
  status: "active" | "completed" | "archived"
): Promise<Project[]> {
  return db.select().from(projects).where(eq(projects.status, status)).all();
}

export async function create(
  data: Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version">
): Promise<Project> {
  const now = new Date();
  const project: NewProject = {
    ...data,
    id: generateId(),
    createdAt: now,
    updatedAt: now,
    version: 1,
  };

  await db.insert(projects).values(project);
  await createOutboxEntry("project", project.id, "create", project);

  return project as Project;
}

export async function update(
  id: string,
  data: Partial<
    Omit<NewProject, "id" | "createdAt" | "updatedAt" | "version" | "createdBy">
  >
): Promise<Project | undefined> {
  const existing = await getById(id);
  if (!existing) return undefined;

  const updated = {
    ...data,
    updatedAt: new Date(),
    version: existing.version + 1,
  };

  await db.update(projects).set(updated).where(eq(projects.id, id));
  await createOutboxEntry("project", id, "update", { ...existing, ...updated });

  return { ...existing, ...updated } as Project;
}

export async function remove(id: string): Promise<void> {
  await db.delete(projects).where(eq(projects.id, id));
  await createOutboxEntry("project", id, "delete", { id });
}
