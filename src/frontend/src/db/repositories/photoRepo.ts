import { eq, and } from "drizzle-orm";
import { db } from "../client";
import { photos } from "../schema";
import { generateId } from "../../utils/uuid";
import { createOutboxEntry } from "./syncRepo";
import type { Photo, NewPhoto } from "./types";
import type { PhotoId, InstallationId } from "../../types/branded";

export async function getByInstallationId(
  installationId: InstallationId
): Promise<Photo[]> {
  return db
    .select()
    .from(photos)
    .where(eq(photos.installationId, installationId))
    .all() as unknown as Photo[];
}

export async function getById(id: PhotoId): Promise<Photo | undefined> {
  return db.select().from(photos).where(eq(photos.id, id)).get() as unknown as Photo | undefined;
}

export async function getPendingUpload(): Promise<Photo[]> {
  return db
    .select()
    .from(photos)
    .where(eq(photos.uploadStatus, "pending"))
    .all() as unknown as Photo[];
}

export async function getFailedUpload(): Promise<Photo[]> {
  return db
    .select()
    .from(photos)
    .where(eq(photos.uploadStatus, "failed"))
    .all() as unknown as Photo[];
}

export async function create(
  data: Omit<NewPhoto, "id" | "version">
): Promise<Photo> {
  const photo: NewPhoto = {
    ...data,
    id: generateId(),
    version: 1,
  };

  await db.insert(photos).values(photo);
  await createOutboxEntry("photo", photo.id, "create", photo);

  return photo as unknown as Photo;
}

export async function updateUploadStatus(
  id: PhotoId,
  uploadStatus: "pending" | "uploading" | "uploaded" | "failed",
  remotePath?: string
): Promise<void> {
  const updates: Record<string, unknown> = { uploadStatus };
  if (remotePath) updates.remotePath = remotePath;

  await db.update(photos).set(updates).where(eq(photos.id, id));
}

export async function remove(id: PhotoId): Promise<void> {
  await db.delete(photos).where(eq(photos.id, id));
  await createOutboxEntry("photo", id, "delete", { id });
}

export async function updateAnnotation(
  id: PhotoId,
  annotation: string
): Promise<void> {
  await db
    .update(photos)
    .set({ annotations: annotation })
    .where(eq(photos.id, id));
  const photo = await getById(id);
  if (photo) {
    await createOutboxEntry("photo", id, "update", photo);
  }
}

export async function getCount(): Promise<number> {
  const rows = await db.select().from(photos).all();
  return rows.length;
}
